using System;
using System.Runtime.Serialization;
using Cortside.Common.Messages;

namespace Acme.IdentityServer.WebApi.Exceptions {
    [Serializable]
    public class InvalidEmailMessage : MessageException {
        public InvalidEmailMessage() : base("Invalid email address provided.  Acme email addresses are not allowed.") { }
        protected InvalidEmailMessage(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
