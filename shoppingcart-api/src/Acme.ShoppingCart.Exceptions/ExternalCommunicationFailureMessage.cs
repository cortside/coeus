using System;
using System.Runtime.Serialization;
using Cortside.Common.Messages.MessageExceptions;

namespace Acme.ShoppingCart.Exceptions {
    [Serializable]
    public class ExternalCommunicationFailureMessage : InternalServerErrorResponseException {
        public ExternalCommunicationFailureMessage(string message) : base(message) {
        }

        public ExternalCommunicationFailureMessage() : base("error communicating with an external service.") {
        }

        protected ExternalCommunicationFailureMessage(SerializationInfo info, StreamingContext context) : base(info, context) {
        }

        public ExternalCommunicationFailureMessage(string message, Exception exception) : base(message, exception) {
        }
    }
}
