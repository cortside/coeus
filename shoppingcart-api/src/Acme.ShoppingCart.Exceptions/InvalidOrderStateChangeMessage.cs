using Cortside.Common.Messages.MessageExceptions;

namespace Acme.ShoppingCart.Exceptions {
    public class InvalidOrderStateChangeMessage : UnprocessableEntityResponseException {
        public InvalidOrderStateChangeMessage() : base("Current state does not allow requested operation.") {
        }

        public InvalidOrderStateChangeMessage(string message) : base($"Current state does not allow requested operation. {message}") {
        }
    }
}
