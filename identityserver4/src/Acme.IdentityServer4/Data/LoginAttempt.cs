using System;
using System.Collections.Generic;

namespace Cortside.IdentityServer.Data {
    public class LoginAttempt {
        public int Id { get; set; }
        public DateTime AttemptedOn { get; set; }
        public bool Successful { get; set; }
        public Guid UserId { get; set; }
        public User User { set; get; }
    }
}
