namespace Acme.IdentityServer.WebApi.Data {
    public class ClientPostLogoutRedirectUri {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string PostLogoutRedirectUri { get; set; } = ""; // default value should not be null
    }
}
