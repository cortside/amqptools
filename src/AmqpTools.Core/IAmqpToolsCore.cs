using System.Collections.Generic;
using System.Threading.Tasks;
using AmqpTools.Core.Commands.DeleteMessage;
using AmqpTools.Core.Commands.Peek;
using AmqpTools.Core.Commands.Queue;
using AmqpTools.Core.Commands.Shovel;
using AmqpTools.Core.Models;

namespace AmqpTools.Core {
    public interface IAmqpToolsCore {
        Task<bool> DeleteMessage(DeleteMessageOptions options);

        Task<AmqpToolsQueueRuntimeInfo> GetQueueRuntimeInfo(QueueOptions options);

        Task ShovelMessages(ShovelOptions options);

        Task<IList<AmqpToolsMessage>> PeekMessages(PeekOptions options);
    }
}
