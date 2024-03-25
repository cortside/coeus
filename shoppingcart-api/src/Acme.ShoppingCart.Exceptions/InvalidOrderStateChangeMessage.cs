#pragma warning disable S3376 // Attribute, EventArgs, and Exception type names should end with the type being extended

using Cortside.Common.Messages.MessageExceptions;

namespace Acme.ShoppingCart.Exceptions {
    public class InvalidOrderStateChangeMessage : UnprocessableEntityResponseException {
        public InvalidOrderStateChangeMessage() : base("Current state does not allow requested operation.") {
        }

        public InvalidOrderStateChangeMessage(string message) : base($"Current state does not allow requested operation. {message}") {
        }

        public InvalidOrderStateChangeMessage(string message, System.Exception exception) : base(message, exception) {
        }
    }
}
