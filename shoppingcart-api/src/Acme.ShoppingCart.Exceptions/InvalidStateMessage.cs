using System;
using Cortside.Common.Messages.MessageExceptions;

namespace Acme.ShoppingCart.Exceptions {
    [Serializable]
    public class InvalidStateMessage : UnprocessableEntityResponseException {
        public InvalidStateMessage() : base("Unable to process request, the entity is in an invalid state.") {
        }

        public InvalidStateMessage(string message) : base(message) {
        }

        public InvalidStateMessage(string message, Exception exception) : base(message, exception) {
        }
    }
}
