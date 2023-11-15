using System;
using System.Runtime.Serialization;
using Cortside.Common.Messages.MessageExceptions;

namespace Acme.IdentityServer.WebApi.Exceptions {
    [Serializable]
    public class ResourceNotFoundMessage : NotFoundResponseException {
        public ResourceNotFoundMessage() : base("No resource could be found with the provided value(s).") { }
        protected ResourceNotFoundMessage(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
