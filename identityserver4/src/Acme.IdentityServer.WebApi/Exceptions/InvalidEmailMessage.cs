using System;
using System.Runtime.Serialization;
using Acme.IdentityServer.Exceptions.Core;

namespace Acme.IdentityServer.Exceptions {
    [Serializable]
    public class InvalidEmailMessage : MessageException {
        public InvalidEmailMessage() : base("Invalid email address provided.  Acme email addresses are not allowed.") {

        }
        protected InvalidEmailMessage(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
