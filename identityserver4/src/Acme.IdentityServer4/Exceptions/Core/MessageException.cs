using System;
using System.Runtime.Serialization;

namespace Cortside.IdentityServer.Exceptions.Core {
    public abstract class MessageException : Exception {
        protected MessageException() : base() { }
        protected MessageException(string message) : base(message) { }
        protected MessageException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
