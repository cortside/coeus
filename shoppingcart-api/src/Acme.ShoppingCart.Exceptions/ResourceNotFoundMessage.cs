using Cortside.Common.Messages.MessageExceptions;

namespace Acme.ShoppingCart.Exceptions {
    public class ResourceNotFoundMessage : NotFoundResponseException {
        public ResourceNotFoundMessage() : base("Resource could not be found.") {
        }

        public ResourceNotFoundMessage(string message) : base(message) {
        }
    }
}
