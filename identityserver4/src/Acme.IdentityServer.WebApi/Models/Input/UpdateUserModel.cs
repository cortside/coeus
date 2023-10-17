using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Acme.IdentityServer.WebApi.Models.Enumerations;

namespace Acme.IdentityServer.WebApi.Models.Input {
    public class UpdateUserModel {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public UserStatus? Status { get; set; }

        /// <summary>
        /// List of user claims
        /// </summary>
        public List<UserClaimModel> Claims { get; set; }
    }
}
