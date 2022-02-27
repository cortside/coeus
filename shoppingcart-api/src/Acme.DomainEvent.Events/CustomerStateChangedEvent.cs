using System;

namespace Acme.DomainEvent.Events {
    public class CustomerStateChangedEvent {
        public Guid CustomerResourceId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
