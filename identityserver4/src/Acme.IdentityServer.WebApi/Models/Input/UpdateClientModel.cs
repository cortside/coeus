using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.IdentityServer.WebApi.Models.Input {
    public class UpdateClientModel {
        [Required]
        public string ClientName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public bool Enabled { get; set; }
        public List<string> Scopes { get; set; }
        public List<UserClaimModel> Claims { get; set; }
    }
}
