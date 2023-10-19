namespace Acme.IdentityServer.WebApi.Models {
    public class UpdateClientRequest {
        public int AccessTokenType { get; set; }

        public string ClientName { get; set; }

        public bool EnableLocalLogin { get; set; }

        public string GrantType { get; set; } // implicit, hybrid, authorization_code, client_credentials, password

        public string[] PostLogoutRedirectUris { get; set; }

        public string[] RedirectUris { get; set; }

        public string[] CorsOrigins { get; set; }

        public string[] Scopes { get; set; }
    }
}
