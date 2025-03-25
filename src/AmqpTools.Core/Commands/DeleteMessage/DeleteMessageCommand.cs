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
                opts.ApplyConfig();
            });

            if (!string.IsNullOrWhiteSpace(result.Value.Environment) && config.Environments.Exists(x => x.Name == result.Value.Environment)) {
                var env = config.Environments.First(x => x.Name == result.Value.Environment);
                Logger.LogInformation("Environment {Env} found in config, using environment settings", env.Name);
                result.Value.Namespace ??= env.Namespace;
                result.Value.PolicyName ??= env.PolicyName;
                result.Value.Key ??= env.Key;
                result.Value.Protocol ??= env.Protocol;
            }

            if (result.Errors.Any()) {
                foreach (var error in result.Errors) {
                    Logger.LogInformation(error.ToString());
                }

                return;
            }

            options = result.Value;
        }

        public async Task<int> ExecuteAsync() {
            Logger.LogInformation($"Connecting to {options.Namespace} as policy {options.PolicyName} for queue {options.Queue}");
            var result = await DeleteMessage();
            return Constants.EXIT_SUCCESS;
        }

        internal async Task<bool> DeleteMessage() {
            var success = false;

            string formattedQueue = EntityNameHelper.FormatQueue(options.Queue, options.MessageType);
            Logger.LogInformation("Delete Message {MessageId} messages from {FormattedQueue}.", options.MessageId, formattedQueue);

            Logger.LogInformation("Attempting to delete message {MessageId}", options.MessageId);

            var counts = await GetQueue(options);
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
                    Logger.LogInformation("Reading message {MessageId}", message.Properties.MessageId);
                    if (message.Properties.MessageId == options.MessageId) {
                        receiver.Accept(message);
                        success = true;
                        Logger.LogInformation("Successfully deleted message {MessageId}", message.Properties.MessageId);
                    } else {
                        messages.Add(message);
                    }
                }
                Logger.LogInformation("releasing {Count} messages", messages.Count);
                foreach (var msg in messages) {
                    receiver.Release(msg);
                }
                if (!success) {
                    throw new NotFoundResponseException($"Message {options.MessageId} could not be found.");
                }

                Logger.LogInformation("Closing connection");
                await receiver.CloseAsync();
                await conn.Session.CloseAsync();
                await conn.Connection.CloseAsync();
                Logger.LogInformation("Connection closed");
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

                Logger.LogInformation("Connection successfully established.");
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

        private async Task<QueueRuntimeProperties> GetQueue(DeleteMessageOptions opts) {
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
