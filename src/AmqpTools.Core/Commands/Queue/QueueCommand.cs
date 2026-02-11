using System;
using System.Linq;
using System.Threading.Tasks;
using AmqpTools.Core.Exceptions;
using AmqpTools.Core.Models;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using CommandLine;
using Cortside.Common.Messages.MessageExceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AmqpTools.Core.Commands.Queue {
    public class QueueCommand : ICommand {
        private QueueOptions options;

        public QueueCommand() {
        }

        public QueueCommand(ILogger logger, QueueOptions options) {
            this.options = options;
            Logger = logger;
        }

        public ILogger Logger { get; set; }

        public void ParseArguments(string[] args, Configuration config) {
            var result = Parser.Default.ParseArguments<QueueOptions>(args);
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
            Logger.LogDebug($"Connecting to {options.Namespace} as policy {options.PolicyName} for queue {options.Queue}");

            var details = await GetQueueRuntimeInfoAsync();
            await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(details, new JsonSerializerSettings() { Formatting = Formatting.Indented }));

            return Constants.EXIT_SUCCESS;
        }

        internal async Task<AmqpToolsQueueRuntimeInfo> GetQueueRuntimeInfoAsync() {
            try {
                var adminClient = new ServiceBusAdministrationClient(options.GetConnectionString());
                var queue = await adminClient.GetQueueRuntimePropertiesAsync(options.Queue);
                return Map(queue.Value);
            } catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound) {
                Logger.LogError(ex, "Error getting queue info: {Message}", ex.Message);
                throw new NotFoundResponseException($"Queue not found {options.Queue}");
            } catch (Exception ex) {
                Logger.LogError(ex, "Error getting queue runtime info {Message}", ex.Message);
                throw new AmqpConnectionException();
            }
        }

        private AmqpToolsQueueRuntimeInfo Map(QueueRuntimeProperties queue) {
            return new AmqpToolsQueueRuntimeInfo {
                Path = queue.Name,
                MessageCount = queue.TotalMessageCount,
                MessageCountDetails = new AmqpToolsMessageCountDetails {
                    ActiveMessageCount = queue.ActiveMessageCount,
                    DeadLetterMessageCount = queue.DeadLetterMessageCount,
                    ScheduledMessageCount = queue.ScheduledMessageCount,
                    TransferMessageCount = queue.TransferMessageCount,
                    TransferDeadLetterMessageCount = queue.TransferDeadLetterMessageCount,
                },
                SizeInBytes = queue.SizeInBytes,
                CreatedAt = queue.CreatedAt.DateTime,
                UpdatedAt = queue.UpdatedAt.DateTime,
                AccessedAt = queue.AccessedAt.DateTime
            };
        }
    }
}
