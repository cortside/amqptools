using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmqpTools.Core.Exceptions;
using AmqpTools.Core.Models;
using AmqpTools.Core.Util;
using Azure.Messaging.ServiceBus;
using CommandLine;
using Cortside.Common.Messages.MessageExceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AmqpTools.Core.Commands.Peek {
    public class PeekCommand : ICommand {
        private PeekOptions options;

        public PeekCommand() {
        }

        public PeekCommand(ILogger logger, PeekOptions options) {
            this.Logger = logger;
            this.options = options;
        }

        public ILogger Logger { get; set; }

        public void ParseArguments(string[] args, Configuration config) {
            var result = Parser.Default.ParseArguments<PeekOptions>(args);
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
            Logger.LogInformation("Connecting to {Namespace} as policy {PolicyName} for queue {Queue}", options.Namespace, options.PolicyName, options.Queue);

            if (options != null) {
                var messages = await PeekMessages();
                Logger.LogInformation("Peeked {Count} messages", messages.Count);
                foreach (var message in messages) {
                    Logger.LogDebug(JsonConvert.SerializeObject(message));
                }

                Logger.LogInformation("Peek complete");
                return Constants.EXIT_SUCCESS;
            } else {
                Logger.LogError("No options set");
                return Constants.ERROR_NO_MESSAGE;
            }
        }

        internal async Task<IList<AmqpToolsMessage>> PeekMessages() {
            string formattedQueue = EntityNameHelper.FormatQueue(options.Queue, options.MessageType);
            Logger.LogInformation("Peeking {Count} messages from `{FormattedQueue}`", options.Count, formattedQueue);

            ServiceBusClient client = new(options.GetConnectionString());

            ServiceBusReceiver receiver = null;
            List<ServiceBusReceivedMessage> messages;
            try {
                receiver = client.CreateReceiver(formattedQueue, new ServiceBusReceiverOptions {
                    ReceiveMode = ServiceBusReceiveMode.PeekLock
                });
                Logger.LogInformation($"receiver created for {formattedQueue}");

                var list = await receiver.PeekMessagesAsync(options.Count);
                Logger.LogInformation($"{list.Count}");
                messages = list.ToList();
                Logger.LogInformation($"{messages.Count}");
            } catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound) {
                Logger.LogError(ex, "Error peeking messages: {Message}", ex.Message);
                throw new NotFoundResponseException($"Queue not found {options.Queue}");
            } catch (Exception ex) {
                Logger.LogError(ex, "Error peeking messages: {Message}", ex.Message);
                throw new AmqpPeekException();
            } finally {
                if (receiver != null) {
                    await receiver.CloseAsync();
                }
            }

            Logger.LogInformation($"{messages.Count} messages peeked");
            return messages.ConvertAll(Map);
        }

        private AmqpToolsMessage Map(ServiceBusReceivedMessage message) {
            string body = AmqpMessageHandler.GetBody(message);

            if (body != null) {
                List<string> errors = new List<string>();
                JsonConvert.DeserializeObject(body,
                    new JsonSerializerSettings {
                        Error = (_, args) => {
                            errors.Add(args.ErrorContext.Error.Message);
                            args.ErrorContext.Handled = true;
                        }
                    });
                if (errors.Count > 0) {
                    Logger.LogWarning("Message {MessageId} has errors deserializing message body: {Errors}", message.MessageId, string.Join(", ", errors));
                }
            }

            var raw = message.GetRawAmqpMessage();
            var systemProperties = raw.DeliveryAnnotations.Concat(raw.MessageAnnotations
                .Where(kvp => !raw.DeliveryAnnotations.ContainsKey(kvp.Key)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return new AmqpToolsMessage {
                Body = body,
                MessageId = message.MessageId,
                CorrelationId = message.CorrelationId,
                PartitionKey = message.PartitionKey,
                ExpiresAtUtc = message.ExpiresAt.DateTime,
                ContentType = message.ContentType,
                ScheduledEnqueueTimeUtc = message.ScheduledEnqueueTime.DateTime,
                UserProperties = message.ApplicationProperties,
                SystemProperties = systemProperties,
            };
        }
    }
}
