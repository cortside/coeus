using System;
using Newtonsoft.Json;

namespace EnerBank.DomainEvent.Events {
    public class BaseMessageRequestEvent {
        [JsonProperty("to")]
        public string[] Recipients { get; set; }

        [JsonProperty("message")]
        public string MessagePayload { get; set; }

        [JsonProperty("replyTo")]
        public Uri ReplyToUrl { get; set; }

        [JsonProperty("internalReferenceId")]
        public string InternalReferenceId { get; set; }
    }
}
