using System;

namespace Acme.IdentityServer.WebApi.Data {
    public class LoginAttempt {
        public int Id { get; set; }
        public DateTime AttemptedOn { get; set; }
        public string IpAddress { get; set; }
        public bool Successful { get; set; }
        public Guid UserId { get; set; }
        public User User { set; get; }
    }
}
