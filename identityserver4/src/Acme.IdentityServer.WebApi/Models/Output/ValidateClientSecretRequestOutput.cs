namespace Acme.IdentityServer.WebApi.Models.Output {
    public class ValidateClientSecretRequestOutput {
        public bool IsValid { get; set; }

        public ClientSecretRequestDecodedOutput DecodedOutput { get; set; }

        public string Last4PhoneNumber { get; set; }
    }
}
