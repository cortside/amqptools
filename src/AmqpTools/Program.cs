using System;
using System.IO;
using System.Threading.Tasks;
using AmqpTools.Core;
using AmqpTools.Core.Commands;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AmqpTools {
    public class Program {
        private static ILogger<Program> logger;
        public static async Task<int> Main(string[] args) {
            if (args.Length == 0 || args[0] == "help" || args[0] == "--help" || args[0] == "-help") {
                Console.Out.WriteLine(CommandFactory.HelpText);
                return Constants.ERROR_NO_MESSAGE;
            }

            var parser = new Parser(with => {
                with.IgnoreUnknownArguments = true;
            });
            var result = parser.ParseArguments<BaseOptions>(args);

            await result
                .WithNotParsed(x => {
                    var help = HelpText.AutoBuild(result, x => x, x => x);
                    Console.WriteLine(help);
                })
                .WithParsedAsync(async x => {
                    if (string.IsNullOrWhiteSpace(result.Value.Config)) {
                        var directory = Directory.GetCurrentDirectory();
                        var filename = Path.Combine(directory, "amqptools.json");
                        result.Value.Config = filename;
                    }

                    var config = new Configuration();
                    if (File.Exists(result.Value.Config)) {
                        var c = new ConfigurationBuilder()
                            .AddJsonFile(result.Value.Config, true)
                            .Build();

                        config = c.Get<Configuration>();
                    }

                    var loggerFactory = LoggerFactory.Create(builder => {
                        builder
                            .AddFilter("Microsoft", LogLevel.Warning)
                            .AddFilter("System", LogLevel.Warning)
                            .SetMinimumLevel(result.Value.Verbose ? LogLevel.Debug : LogLevel.Critical)
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
