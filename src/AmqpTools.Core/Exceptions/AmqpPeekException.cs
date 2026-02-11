using Cortside.Common.Messages.MessageExceptions;

namespace AmqpTools.Core.Exceptions {
    public class AmqpPeekException : InternalServerErrorResponseException {
        public AmqpPeekException() : base("Failed to peek messages in queue.") { }

        public AmqpPeekException(string message) : base(message) {
        }

        public AmqpPeekException(string message, System.Exception exception) : base(message, exception) {
        }

        protected AmqpPeekException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected AmqpPeekException(string message, string property) : base(message, property) {
        }
    }
}
