using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        { 
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope()
            {
                Name = "shoppingcart-service",
                Description = "shoppingcart-service",
                DisplayName = "shoppingcart-service",
                Required = false,
                ShowInDiscoveryDocument = true
            }
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("shoppingcart-service","shoppingcart-service")
            {
                ApiSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },
                Scopes = new List<string>() { "shoppingcart-service" },
            }
        };

    public static IEnumerable<Client> Clients =>
        new List<Client> 
        {
            // machine-to-machine client (from quickstart 1)
            new Client
            {
                ClientId = "shoppingcart-web",
                ClientName = "shoppingcart-web",
                AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                AllowOfflineAccess = true,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowAccessTokensViaBrowser = true,
                RedirectUris = { "http://localhost:4200/login-redirect", "http://localhost:4200/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:4200/" },
                FrontChannelLogoutUri = "http://localhost:4200/signout-oidc",

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "shoppingcart-service"
                },
            },
            // interactive ASP.NET Core Web App
            new Client
            {
                ClientId = "shoppingcart-service",
                ClientName = "shoppingcart-service",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes =
                {
                    "shoppingcart-service"
                },
            }
        };
}