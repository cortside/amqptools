using Cortside.Common.Messages.MessageExceptions;

namespace AmqpTools.Core.Exceptions {
    public class AmqpShovelException : InternalServerErrorResponseException {
        public AmqpShovelException() : base("Failed to shovel messages.") { }

        public AmqpShovelException(string message) : base(message) {
        }

        public AmqpShovelException(string message, System.Exception exception) : base(message, exception) {
        }

        protected AmqpShovelException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected AmqpShovelException(string message, string property) : base(message, property) {
        }
    }
}
