using System;

namespace Acme.IdentityServer.WebApi.Data {
    //Limitation of EFCore 1.1 - This class should not exist.
    public class UserClaim {
        public int UserClaimId { set; get; }
        public Guid UserId { set; get; }
        public User User { set; get; }
        public string ProviderName { set; get; }

        //The following two fields represent the key to value pair of the UserClaim
        public string Type { set; get; }
        public string Value { set; get; }
    }
}
