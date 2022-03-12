using System;

// TODO: fix namespace or registration

namespace Acme.DomainEvent.Events {
    public class CustomerStateChangedEvent {
        public Guid CustomerResourceId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
