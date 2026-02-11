using System.Collections.Generic;
using System.Threading.Tasks;
using AmqpTools.Core.Commands;
using AmqpTools.Core.Commands.DeleteMessage;
using AmqpTools.Core.Commands.Peek;
using AmqpTools.Core.Commands.Shovel;
using AmqpTools.Core.Models;

namespace AmqpTools.Core {
    public interface IAmqpToolsCore {
        Task<bool> DeleteMessageAsync(DeleteMessageOptions options);

        Task<AmqpToolsQueueRuntimeInfo> GetQueueRuntimeInfoAsync(QueueOptions options);

        Task ShovelMessagesAsync(ShovelOptions options);

        Task<IList<AmqpToolsMessage>> PeekMessagesAsync(PeekOptions options);
    }
}
