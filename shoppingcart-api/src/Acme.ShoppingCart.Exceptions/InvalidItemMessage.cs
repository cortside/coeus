using Cortside.Common.Messages.MessageExceptions;

namespace Acme.ShoppingCart.Exceptions {
    public class InvalidItemMessage : BadRequestResponseException {
        public InvalidItemMessage() : base("Item is not valid.") {
        }

        public InvalidItemMessage(string message) : base($"Item is not valid. {message}") {
        }
    }
}
