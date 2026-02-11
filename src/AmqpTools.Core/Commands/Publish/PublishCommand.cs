using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AmqpTools.Core.Exceptions;
using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AmqpTools.Core.Commands.Publish {
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
    public class PublishCommand : ICommand {
        private PublishOptions options;

        public ILogger Logger { get; set; }

        public PublishCommand() { }

        public PublishCommand(ILogger logger, PublishOptions options) {
            Logger = logger;
            this.options = options;
        }

        public void ParseArguments(string[] args, Configuration config) {
            var result = Parser.Default.ParseArguments<PublishOptions>(args);
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
            var result = await PublishMessageAsync();
            return result ? Constants.EXIT_SUCCESS : Constants.ERROR_NO_MESSAGE;
        }

        internal async Task<bool> PublishMessageAsync() {
            var handler = new AmqpMessageHandler(Logger, options);

            if (!string.IsNullOrWhiteSpace(options.File)) {
                options.Data = (await File.ReadAllBytesAsync(options.File)).ToString();
            }

            if (string.IsNullOrEmpty(options.Data)) {
                Logger.LogError("Data or File must be specified and have data");
                throw new InvalidArgumentMessageException($"Data or File option must be specified and have data");
            }

            var message = AmqpMessageHandler.CreateMessage(options.EventType, options.Data, null);
            bool success;
            try {
                handler.Send(message);
                await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(message, Formatting.Indented));
                success = true;
            } catch (Exception ex) {
                Logger.LogError(ex, "Error publishing message");
                success = false;
            }
            if (success) {
                Logger.LogDebug("message sent");
            }

            return success;
        }
    }
}
