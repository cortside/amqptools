using System;
using System.IO;
using System.Threading.Tasks;
using AmqpTools.Core.Commands;
using AmqpTools.Core.Commands.DeleteMessage;
using AmqpTools.Core.Commands.Peek;
using AmqpTools.Core.Commands.Publish;
using AmqpTools.Core.Commands.Queue;
using AmqpTools.Core.Commands.Shovel;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AmqpTools {
    public class Program {
        private static ILogger<Program> logger;
        public static async Task<int> Main(string[] args) {
            if (args.Length == 0) {
                throw new ArgumentException("Must pass arguments", nameof(args));
            }

            var directory = Directory.GetCurrentDirectory();
            var filename = Path.Combine(directory, "amqptools.json");

            var config = new Configuration();
            if (File.Exists(filename)) {
                var c = new ConfigurationBuilder()
                    .AddJsonFile(filename, true)
                    .Build();

                config = c.Get<Configuration>();
            }

            var result = CommandLine.Parser.Default.ParseArguments<PeekOptions, PublishOptions, QueueOptions, DeleteMessageOptions, ShovelOptions>(args);
            result
                .WithNotParsed(x => {
                    Console.WriteLine("command type not parsed");
                })
                .WithParsedAsync(async x => {
                    Console.WriteLine($"parsed type result: {x}");

                    var loggerFactory = LoggerFactory.Create(builder => {
                        builder
                            .AddFilter("Microsoft", LogLevel.Warning)
                            .AddFilter("System", LogLevel.Warning)
                            .SetMinimumLevel(LogLevel.Debug)
                            .AddConsole();
                    });

                    var command = new CommandFactory().CreateCommand(loggerFactory, args, config);
                    if (command != null) {
                        await command.ExecuteAsync();
                    }
                });

            return 1;
        }
    }
}
