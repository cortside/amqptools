using AmqpTools.Core.Commands;
using AmqpTools.Core.Commands.Publish;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace AmqpTools.Test {
    public class CommandFactoryTest : BaseTest {
        private readonly ILoggerFactory loggerFactory;

        public CommandFactoryTest() : base() {
            loggerFactory = LoggerFactory.Create(builder => {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .SetMinimumLevel(LogLevel.Debug);
                //.AddConsole();
            });
        }

        [Fact]
        public void ShouldThrowOnUnknownCommand() {
            var x = new CommandFactory().CreateCommand(loggerFactory, new string[] { "blah" }, new Configuration());
            x.ShouldBeNull("unknown command");
        }

        [Fact]
        public void ShouldCreateCommand() {
            // Act
            var command = new CommandFactory().CreateCommand(loggerFactory, new string[] { "publish" }, new Configuration());

            // assert
            command.ShouldNotBeNull();
            command.ShouldBeOfType<PublishCommand>();
            command.Logger.ShouldNotBeNull();
        }
    }
}
