using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cortside.IdentityServer.Exceptions.Core {
    public abstract class MessageException : Exception {
        protected MessageException() : base() { }
        protected MessageException(string message) : base(message) { }
        protected MessageException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
