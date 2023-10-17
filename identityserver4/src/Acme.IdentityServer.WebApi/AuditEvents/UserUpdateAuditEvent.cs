using IdentityServer4.Events;

namespace Acme.IdentityServer.WebApi.Events {
    public class UserUpdateAuditEvent : Event {
        public UserUpdateAuditEvent(string username) : base(EventCategories.Authentication, "User Create Success", EventTypes.Success, EventIds.UserLoginSuccess) {
            Username = username;
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }
    }
}
