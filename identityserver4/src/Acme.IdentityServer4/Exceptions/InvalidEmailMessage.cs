using System;
using System.Runtime.Serialization;
using Cortside.IdentityServer.Exceptions.Core;

namespace Cortside.IdentityServer.Exceptions {
    [Serializable]
    public class InvalidEmailMessage : MessageException {
        public InvalidEmailMessage() : base("Invalid email address provided.  Cortside email addresses are not allowed.") {

        }
        protected InvalidEmailMessage(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
