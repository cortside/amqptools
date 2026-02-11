using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmqpTools.Core.Commands.DeleteMessage;
using AmqpTools.Core.Commands.Peek;
using AmqpTools.Core.Commands.Publish;
using AmqpTools.Core.Commands.Queue;
using AmqpTools.Core.Commands.Shovel;
using Microsoft.Extensions.Logging;

namespace AmqpTools.Core.Commands {
    public class CommandFactory {
        private static readonly Dictionary<string, Type> commands;

        static CommandFactory() {
            commands = new Dictionary<string, Type>() {
                {"queue", typeof(QueueCommand)},
                {"shovel", typeof(ShovelCommand)},
                {"peek", typeof(PeekCommand)},
                {"delete", typeof(DeleteMessageCommand)},
                {"publish", typeof(PublishCommand)}
            };
        }

        public static List<string> Commands => commands.Keys.ToList();

        public ICommand CreateCommand(ILoggerFactory loggerFactory, string[] args, Configuration config) {
            var name = args[0];
            if (!commands.TryGetValue(name, out Type value)) {
                Console.Out.WriteLine(HelpText);
                return null;
            }

            var type = value;
            var command = Activator.CreateInstance(type) as ICommand;
            command.Logger = loggerFactory.CreateLogger(type);
            command.ParseArguments(args, config);

            return command;
        }

        public static string HelpText {
            get {
                var sb = new StringBuilder();
                sb.AppendLine("ERROR(S): ");
                sb.AppendLine($"Valid commands are: {string.Join(',', CommandFactory.Commands)}");
                sb.AppendLine("Use `dotnet amqptools <command> --help` for command help");

                return sb.ToString();
            }
        }
    }
}
