using Cortside.Common.Messages.MessageExceptions;

namespace AmqpTools.Core.Exceptions {
    public class AmqpConnectionException : InternalServerErrorResponseException {
        public AmqpConnectionException() : base("Could not establish a connection to the server.") { }

        public AmqpConnectionException(string message) : base(message) {
        }

        public AmqpConnectionException(string message, System.Exception exception) : base(message, exception) {
        }

        protected AmqpConnectionException(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected AmqpConnectionException(string message, string property) : base(message, property) {
        }
    }
}
