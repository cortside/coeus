#pragma warning disable S3376 // Attribute, EventArgs, and Exception type names should end with the type being extended

using Cortside.Common.Messages.MessageExceptions;

namespace Acme.ShoppingCart.Exceptions {
    public class InvalidItemMessage : BadRequestResponseException {
        public InvalidItemMessage() : base("Item is not valid.") {
        }

        public InvalidItemMessage(string message) : base($"Item is not valid. {message}") {
        }

        public InvalidItemMessage(string message, System.Exception exception) : base(message, exception) {
        }

        protected InvalidItemMessage(string key, string property, params object[] properties) : base(key, property, properties) {
        }

        protected InvalidItemMessage(string message, string property) : base(message, property) {
        }
    }
}
