namespace Acme.IdentityServer.WebApi.Data {
    public class ClientGrantType {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string GrantType { get; set; } = ""; // default value should not be null
    }
}
