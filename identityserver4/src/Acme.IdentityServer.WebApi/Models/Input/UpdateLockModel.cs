namespace Acme.IdentityServer.WebApi.Models.Input {
    public class UpdateLockModel {
        /// <summary>
        /// The locked state of a user
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Why the user was locked
        /// </summary>
        public string Reason { get; set; }
    }
}
