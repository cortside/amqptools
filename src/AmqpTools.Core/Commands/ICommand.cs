using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AmqpTools.Core.Commands {
    public interface ICommand {
        void ParseArguments(string[] args, Configuration config);
        Task<int> ExecuteAsync();
        ILogger Logger { get; set; }
    }
}
