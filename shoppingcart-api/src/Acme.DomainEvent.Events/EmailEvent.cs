namespace Acme.DomainEvent.Events {

    public class EmailEvent {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Body { get; set; }
    }
}
