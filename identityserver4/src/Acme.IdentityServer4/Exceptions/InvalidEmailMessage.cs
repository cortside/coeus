using System;
using System.Runtime.Serialization;
using EnerBank.IdentityServer.Exceptions.Core;

namespace EnerBank.IdentityServer.Exceptions {
    [Serializable]
    public class InvalidEmailMessage : MessageException {
        public InvalidEmailMessage() : base("Invalid email address provided.  EnerBank email addresses are not allowed.") {

        }
        protected InvalidEmailMessage(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
