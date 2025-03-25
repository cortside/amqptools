using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Task ShovelMessages(ShovelOptions options) {
            var command = new ShovelCommand(factory.CreateLogger<ShovelCommand>(), options);
            return command.Shovel();
        }

        public async Task<AmqpToolsQueueRuntimeInfo> GetQueueRuntimeInfo(QueueOptions options) {
            var command = new QueueCommand(factory.CreateLogger<QueueCommand>(), options);
            command.Logger = factory.CreateLogger<QueueCommand>();
            var result = await command.GetQueueRuntimeInfo();
            return result;
        }

        public async Task<IList<AmqpToolsMessage>> PeekMessages(PeekOptions options) {
            var command = new PeekCommand(factory.CreateLogger<PeekCommand>(), options);
            command.Logger = factory.CreateLogger<PeekCommand>();
            var result = await command.PeekMessages();
            return result;
        }

        public async Task<bool> DeleteMessage(DeleteMessageOptions options) {
            var command = new DeleteMessageCommand(factory.CreateLogger<DeleteMessageCommand>(), options);
            command.Logger = factory.CreateLogger<DeleteMessageCommand>();
            var result = await command.DeleteMessage();
            return result;
        }
    }
}
