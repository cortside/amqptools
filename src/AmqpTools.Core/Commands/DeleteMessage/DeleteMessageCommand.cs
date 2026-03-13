using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amqp;
using AmqpTools.Core.Exceptions;
using AmqpTools.Core.Util;
using Azure.Messaging.ServiceBus.Administration;
using CommandLine;
using Cortside.Common.Messages.MessageExceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AmqpTools.Core.Commands.DeleteMessage {
    public class DeleteMessageCommand : ICommand {
        private DeleteMessageOptions options;

        public DeleteMessageCommand() { }

        public DeleteMessageCommand(ILogger logger, DeleteMessageOptions options) {
            Logger = logger;
            this.options = options;
        }

        public ILogger Logger { get; set; }

        public void ParseArguments(string[] args, Configuration config) {
            var result = Parser.Default.ParseArguments<DeleteMessageOptions>(args);
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
            var result = await DeleteMessageAsync();
            return Constants.EXIT_SUCCESS;
        }

        internal async Task<bool> DeleteMessageAsync() {
            var success = false;

            string formattedQueue = EntityNameHelper.FormatQueue(options.Queue, options.MessageType);
            Logger.LogDebug("Delete Message {MessageId} messages from {FormattedQueue}.", options.MessageId, formattedQueue);

            Logger.LogDebug("Attempting to delete message {MessageId}", options.MessageId);

            var counts = await GetQueueAsync(options);
            var count = formattedQueue.Contains("deadletter", StringComparison.CurrentCultureIgnoreCase) ? counts.DeadLetterMessageCount : counts.ActiveMessageCount;
            AmqpConnection conn = null;
            List<Amqp.Message> messages = new List<Amqp.Message>();
            Amqp.Message message;
            try {
                conn = Connect();
                ReceiverLink receiver = new ReceiverLink(conn.Session, $"receiver-read", formattedQueue);
                receiver.SetCredit((int)count);
                TimeSpan timeSpan = TimeSpan.FromSeconds(10);
                while ((message = await receiver.ReceiveAsync(timeSpan)) != null) {
                    Logger.LogDebug("Reading message {MessageId}", message.Properties.MessageId);
                    if (message.Properties.MessageId == options.MessageId) {
                        receiver.Accept(message);
                        await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(message, Formatting.Indented));
                        success = true;
                        Logger.LogDebug("Successfully deleted message {MessageId}", message.Properties.MessageId);
                    } else {
                        messages.Add(message);
                    }
                }
                Logger.LogDebug("releasing {Count} messages", messages.Count);
                foreach (var msg in messages) {
                    receiver.Release(msg);
                }
                if (!success) {
                    throw new NotFoundResponseException($"Message {options.MessageId} could not be found.");
                }

                Logger.LogDebug("Closing connection");
                await receiver.CloseAsync();
                await conn.Session.CloseAsync();
                await conn.Connection.CloseAsync();
                Logger.LogDebug("Connection closed");
            } catch (Exception e) {
                if (null != conn?.Connection) {
                    await conn.Connection.CloseAsync();
                }
                Logger.LogError(e, "Error deleting message {MessageId} from queue {Queue}", options.MessageId, options.Queue);
                throw;
            }
            return success;
        }


        internal AmqpConnection Connect() {
            Logger.LogDebug("Connecting to {Url}.", options.Namespace);
            try {
                Address address = new Address(options.GetUrl());
                var connection = new Connection(address);
                Session session = new Session(connection);

                Logger.LogDebug("Connection successfully established.");
                return new AmqpConnection() { Connection = connection, Session = session };
            } catch (Exception ex) {
                Logger.LogError(ex, "ServiceBusClient failed to establish connection.");
                throw new AmqpConnectionException();
            }
        }

        internal class AmqpConnection {
            public Connection Connection { get; set; }
            public Session Session { get; set; }
        }

        private async Task<QueueRuntimeProperties> GetQueueAsync(DeleteMessageOptions opts) {
            try {
                var adminClient = new ServiceBusAdministrationClient(opts.GetConnectionString());
                var queue = await adminClient.GetQueueRuntimePropertiesAsync(opts.Queue);
                return queue.Value;

            } catch (Exception ex) {
                Logger.LogError(ex, "Queue not found {Queue}", opts.Queue);
                throw new NotFoundResponseException($"Queue not found {opts.Queue}");
            }
        }
    }
}
