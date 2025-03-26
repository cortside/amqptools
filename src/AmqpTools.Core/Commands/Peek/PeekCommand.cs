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
                opts.ApplyConfig(config);
            });

            if (result.Errors.Any()) {
                foreach (var error in result.Errors) {
                    Logger.LogError(error.ToString());
                }

                return;
            }

            options = result.Value;
        }

        public async Task<int> ExecuteAsync() {
            if (options != null) {
                Logger.LogDebug("Connecting to {Namespace} as policy {PolicyName} for queue {Queue}", options.Namespace, options.PolicyName, options.Queue);

                var messages = await PeekMessagesAsync();
                await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(messages, new JsonSerializerSettings() { Formatting = Formatting.Indented }));

                Logger.LogDebug("Peek complete");
                return Constants.EXIT_SUCCESS;
            }

            return Constants.ERROR_NO_MESSAGE;
        }

        internal async Task<IList<AmqpToolsMessage>> PeekMessagesAsync() {
            string formattedQueue = EntityNameHelper.FormatQueue(options.Queue, options.MessageType);
            Logger.LogDebug("Peeking {Count} messages from `{FormattedQueue}`", options.Count, formattedQueue);

            ServiceBusClient client = new(options.GetConnectionString());

            ServiceBusReceiver receiver = null;
            List<ServiceBusReceivedMessage> messages;
            try {
                receiver = client.CreateReceiver(formattedQueue, new ServiceBusReceiverOptions {
                    ReceiveMode = ServiceBusReceiveMode.PeekLock
                });
                Logger.LogDebug($"receiver created for {formattedQueue}");

                var list = await receiver.PeekMessagesAsync(options.Count);
                messages = list.ToList();
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

            Logger.LogDebug($"{messages.Count} messages peeked");
            return messages.ConvertAll(Map);
        }

        private AmqpToolsMessage Map(ServiceBusReceivedMessage message) {
            string body = AmqpMessageHandler.GetBody(message);

            object data = null;
            if (body != null) {
                List<string> errors = new List<string>();
                data = JsonConvert.DeserializeObject(body,
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

            var result = new AmqpToolsMessage {
                Content = body,
                Data = data,
                MessageId = message.MessageId,
                CorrelationId = message.CorrelationId,
                PartitionKey = message.PartitionKey,
                ExpiresAtUtc = message.ExpiresAt.DateTime,
                ContentType = message.ContentType,
                ScheduledEnqueueTimeUtc = message.ScheduledEnqueueTime.DateTime,
                ApplicationProperties = message.ApplicationProperties
                //SystemProperties = systemProperties,
            };

            return result;
        }
    }
}
