using Cortside.Common.Messages.MessageExceptions;

namespace AmqpTools.Core.Exceptions {
    public class AmqpMessageParseException : InternalServerErrorResponseException {
        public AmqpMessageParseException() : base("Failed to parse the message.") { }

        public AmqpMessageParseException(string message) : base(message) {
        }

        public AmqpMessageParseException(string message, System.Exception exception) : base(message, exception) {
        }

        protected AmqpMessageParseException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected AmqpMessageParseException(string message, string property) : base(message, property) {
        }
    }
}
