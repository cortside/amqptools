using CommandLine;

namespace AmqpTools.Core.Commands {
    [Verb("queue", HelpText = "gets runtime info for a queue")]
    public class QueueOptions : BaseOptions {
        [Option('q', "queue", Required = true, HelpText = "Queue")]
        public string Queue { get; set; }
    }
}
