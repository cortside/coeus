namespace Acme.IdentityServer.WebApi.Models.Output {
    public class IsVerificationCodeValidOutput {

        public bool IsValid { get; set; }

        public string Reason { get; set; }
    }
}
