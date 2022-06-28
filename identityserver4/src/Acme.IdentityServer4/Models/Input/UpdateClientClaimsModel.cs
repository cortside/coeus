using System.Collections.Generic;

namespace EnerBank.IdentityServer.WebApi.Models.Input {
    public class UpdateClientClaimsModel {

        public List<UserClaimModel> Claims { get; set; }
    }
}
