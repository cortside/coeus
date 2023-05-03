using System;
using System.Runtime.Serialization;
using Cortside.Common.Messages.MessageExceptions;

namespace Acme.IdentityServer.WebApi.Exceptions {
    [Serializable]
    public class BadRequestException : BadRequestResponseException {
        public BadRequestException(string message) : base(message) { }
        public BadRequestException() : base("No resource could be found with the provided value(s).") { }
        protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public BadRequestException(string message, Exception exception) : base(message, exception) { }
    }
}
