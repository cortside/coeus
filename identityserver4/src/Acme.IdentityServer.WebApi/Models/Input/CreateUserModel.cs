using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.IdentityServer.WebApi.Models.Input {
    public class CreateUserModel {

        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// List of user claims
        /// </summary>
        public List<UserClaimModel> Claims { get; set; }
    }
}
