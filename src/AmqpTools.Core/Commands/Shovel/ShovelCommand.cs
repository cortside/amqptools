using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amqp;
using AmqpTools.Core.Exceptions;
using AmqpTools.Core.Util;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using CommandLine;
using Cortside.Common.Messages.MessageExceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AmqpTools.Core.Commands.Shovel {
    public class ShovelCommand : ICommand {
        private ShovelOptions options;

        public ILogger Logger { get; set; }

        public ShovelCommand() { }
        public ShovelCommand(ILogger logger, ShovelOptions options) {
            Logger = logger;
            this.options = options;
        }

        public void ParseArguments(string[] args, Configuration config) {
            var result = Parser.Default.ParseArguments<ShovelOptions>(args);
            result.WithParsed(opts => {
                opts.ApplyConfig(config);
            });

            if (result.Errors.Any()) {
                foreach (var error in result.Errors) {
                    Logger.LogDebug(error.ToString());
                }

                return;
            }

            options = result.Value;
        }

        public async Task<int> ExecuteAsync() {
            Logger.LogDebug($"Connecting to {options.Namespace} as policy {options.PolicyName} for queue {options.Queue}");

            var exitCode = await ShovelAsync();
            return exitCode;
        }

        internal async Task<int> ShovelAsync() {
            var handler = new AmqpMessageHandler(Logger, options);

            var dlq = EntityNameHelper.FormatDeadLetterPath(options.Queue);
            var max = options.Max;
            Logger.LogDebug("Connecting to {Queue} to shovel maximum of {Max} messages", options.Queue, max);
            try {
                if (options.GetConnectionString() != null) {
                    var adminClient = new ServiceBusAdministrationClient(options.GetConnectionString());
                    var queue = await adminClient.GetQueueRuntimePropertiesAsync(options.Queue);
                    var messageCount = queue.Value.DeadLetterMessageCount;
                    Logger.LogDebug("Message queue {Dlq} has {MessageCount} messages", dlq, messageCount);

                    if (messageCount < options.Max) {
                        max = Convert.ToInt32(messageCount);
                        Logger.LogDebug("resetting max messages to {Max}", max);
                    }
                }
            } catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound) {
                Logger.LogError(ex, "Error getting queue info: {Message}", ex.Message);
                throw new NotFoundResponseException($"Queue not found {options.Queue}");
            } catch (Exception ex) {
                Logger.LogError(ex, "Error getting queue runtime info {Message}", ex.Message);
                throw new AmqpShovelException();
            }

            int exitCode = Constants.EXIT_SUCCESS;
            Connection connection = null;
            try {
                Address address = new Address(options.GetUrl());
                connection = new Connection(address);
                Session session = new Session(connection);
                ReceiverLink receiver = new ReceiverLink(session, "receiver-drain", dlq);

                Amqp.Message message;
                int nReceived = 0;
                var timeout = options.GetTimeSpan();
                receiver.SetCredit(max);
                var useMessageId = !string.IsNullOrEmpty(options.MessageId);
                List<Amqp.Message> messagesToRelease = new List<Amqp.Message>();

                var messages = new List<Message>();

                while ((message = await receiver.ReceiveAsync(timeout)) != null) {
                    messages.Add(message);

                    nReceived++;
                    var body = AmqpMessageHandler.GetBody(message);
                    Logger.LogDebug("Reading message {MessageId}", message.Properties.MessageId);
                    Logger.LogDebug("Message(Properties={0}, ApplicationProperties={1}, Content={2}", message.Properties, message.ApplicationProperties, body);

                    if (body != null && useMessageId && message.Properties.MessageId == options.MessageId) {
                        Logger.LogDebug("Shoveling single message {MessageId}", options.MessageId);
                        handler.Send(message);
                        receiver.Accept(message);
                    } else if (useMessageId) {
                        messagesToRelease.Add(message);
                    }

                    // TODO: should have option to skip messages that are not valid -- i.e. don't have a type
                    if (body != null && !useMessageId) {
                        handler.Send(message);
                        receiver.Accept(message);
                    }

                    if (options.Max > 0 && nReceived == max) {
                        Logger.LogDebug("max messages received");
                        break;
                    }
                }

                Logger.LogDebug("releasing {Count} messages", messagesToRelease.Count);
                foreach (var msg in messagesToRelease) {
                    Logger.LogDebug("Releasing message {MessageId}, it is not the one to be shoveled", msg.Properties.MessageId);
                    receiver.Release(msg);
                }
                await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(messages.Select(x => x.Properties.MessageId), Formatting.Indented));

                if (message == null) {
                    Logger.LogDebug("No message");
                    exitCode = Constants.ERROR_NO_MESSAGE;
                }
                await receiver.CloseAsync();
                await session.CloseAsync();
                await connection.CloseAsync();
            } catch (Exception e) {
                Logger.LogError(e, "Exception {0}.", e.Message);
                if (null != connection) {
                    await connection.CloseAsync();
                }
                exitCode = Constants.ERROR_OTHER;
                throw new AmqpShovelException();
            }

            Logger.LogDebug("done");
            return exitCode;
        }
    }
}
