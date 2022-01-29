using System;

namespace Acme.DomainEvent.Events {
    public class WidgetStageChangedEvent {
        public int WidgetId { get; set; }

        public string Text { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
