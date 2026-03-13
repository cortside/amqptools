using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AmqpTools.Core.Commands;
using AmqpTools.Core.Commands.DeleteMessage;
using AmqpTools.Core.Commands.Peek;
using AmqpTools.Core.Commands.Queue;
using AmqpTools.Core.Commands.Shovel;
using AmqpTools.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AmqpTools.Core {
    public class AmqpToolsCore : IAmqpToolsCore {
        private readonly ILoggerFactory factory;

        public AmqpToolsCore(IServiceProvider serviceProvider) {
            factory = serviceProvider.GetRequiredService<ILoggerFactory>();
        }

        public Task ShovelMessagesAsync(ShovelOptions options) {
            var command = new ShovelCommand(factory.CreateLogger<ShovelCommand>(), options);
            return command.ShovelAsync();
        }

        public async Task<AmqpToolsQueueRuntimeInfo> GetQueueRuntimeInfoAsync(QueueOptions options) {
            var command = new QueueCommand(factory.CreateLogger<QueueCommand>(), options);
            command.Logger = factory.CreateLogger<QueueCommand>();
            var result = await command.GetQueueRuntimeInfoAsync();
            return result;
        }

        public async Task<IList<AmqpToolsMessage>> PeekMessagesAsync(PeekOptions options) {
            var command = new PeekCommand(factory.CreateLogger<PeekCommand>(), options);
            command.Logger = factory.CreateLogger<PeekCommand>();
            var result = await command.PeekMessagesAsync();
            return result;
        }

        public async Task<bool> DeleteMessageAsync(DeleteMessageOptions options) {
            var command = new DeleteMessageCommand(factory.CreateLogger<DeleteMessageCommand>(), options);
            command.Logger = factory.CreateLogger<DeleteMessageCommand>();
            var result = await command.DeleteMessageAsync();
            return result;
        }
    }
}
