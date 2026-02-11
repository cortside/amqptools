using Cortside.Common.Messages.MessageExceptions;

namespace AmqpTools.Core.Exceptions {
    public class AmqpReceiverException : InternalServerErrorResponseException {
        public AmqpReceiverException() : base("Failed to receive messages.") { }

        public AmqpReceiverException(string message) : base(message) {
        }

        public AmqpReceiverException(string message, System.Exception exception) : base(message, exception) {
        }

        protected AmqpReceiverException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected AmqpReceiverException(string message, string property) : base(message, property) {
        }
    }
}
