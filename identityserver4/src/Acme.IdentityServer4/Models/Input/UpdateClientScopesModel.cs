using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cortside.IdentityServer.WebApi.Models.Input {
    public class UpdateClientScopesModel {

        [Required]
        public List<string> Scopes { get; set; }
    }
}
