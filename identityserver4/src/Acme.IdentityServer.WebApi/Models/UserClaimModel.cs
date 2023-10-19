namespace Acme.IdentityServer.WebApi.Models {
    //Limitation of EFCore 1.1 - This class should not exist.
    public class UserClaimModel {
        public string Type { set; get; }
        public string Value { set; get; }
    }
}
