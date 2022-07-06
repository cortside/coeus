using System;
using System.Runtime.Serialization;
using EnerBank.IdentityServer.Exceptions.Core;

namespace EnerBank.IdentityServer.Exceptions {
    [Serializable]
    public class InvalidValueMessage : MessageException {
        public string Name { get; set; }
        public object Value { get; set; }
        public InvalidValueMessage(string name, object value) : base($"Provided value {value} is invalid for {name}.") {
            this.Name = name;
            this.Value = value;
        }
        protected InvalidValueMessage(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
