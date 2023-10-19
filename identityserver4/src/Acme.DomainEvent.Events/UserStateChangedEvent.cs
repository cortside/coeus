using System;

namespace Acme.DomainEvent.Events {
    public class UserStateChangedEvent {
        public Guid SubjectId { get; set; }
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string UserPrincipalName { get; set; }
    }
}
