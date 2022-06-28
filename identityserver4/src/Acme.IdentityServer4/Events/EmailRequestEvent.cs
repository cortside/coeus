using System;
using Newtonsoft.Json;

namespace Cortside.DomainEvent.Events {
    public class EmailRequestEvent {
        /// <inheritdoc/>
        [JsonProperty("to")]
        public string[] Recipients { get; set; }

        /// <inheritdoc/>
        [JsonProperty("subject")]
        public string Subject { get; set; }

        /// <inheritdoc/>
        [JsonProperty("message")]
        public string MessagePayload { get; set; }

        /// <inheritdoc/>
        [JsonProperty("replyTo")]
        public string ReplyToUrl { get; set; }

        /// <inheritdoc/>
        [JsonProperty("sendAt")]
        public string SendAt { get; set; }

        /// <inheritdoc/>
        [JsonProperty("from")]
        public string From { get; set; }

        /// <inheritdoc/>
        [JsonProperty("internalReferenceId")]
        public Guid InternalReferenceId { get; set; }
    }
}
