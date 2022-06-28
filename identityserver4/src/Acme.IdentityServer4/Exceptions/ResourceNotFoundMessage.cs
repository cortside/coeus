using System;
using System.Runtime.Serialization;
using Cortside.IdentityServer.Exceptions.Core;

namespace Cortside.IdentityServer.Exceptions {
    [Serializable]
    public class ResourceNotFoundMessage : MessageException {
        public ResourceNotFoundMessage() : base("No resource could be found with the provided value(s).") {

        }
        protected ResourceNotFoundMessage(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
