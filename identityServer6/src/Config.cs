using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config {
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource> {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope> {
            new ApiScope() {
                Name = "shoppingcart-api",
                Description = "shoppingcart-api",
                DisplayName = "shoppingcart-api",
                Required = false,
                ShowInDiscoveryDocument = true
            },
            new ApiScope() {
                Name = "catalog-api",
                Description = "catalog-api",
                DisplayName = "catalog-api",
                Required = false,
                ShowInDiscoveryDocument = true
            }
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource> {
            new ApiResource("shoppingcart-api", "ShoppingCart Api") {
                ApiSecrets = new List<Secret> {
                    new Secret("secret".Sha256())
                },
                Scopes = new List<string>() {
                    "shoppingcart-api"
                }
            }
        };

    public static IEnumerable<Client> Clients =>
        new List<Client> {
            new Client {
                ClientId = "shoppingcart-web",
                ClientName = "shoppingcart-web",
                AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                AllowOfflineAccess = true,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowAccessTokensViaBrowser = true,
                RedirectUris = { "http://localhost:4200/login-redirect", "http://localhost:4200/signin-oidc" },
                PostLogoutRedirectUris = { "http://localhost:4200/" },
                FrontChannelLogoutUri = "http://localhost:4200/signout-oidc",

                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "shoppingcart-api",
                    "catalog-api"
                },
            },
            new Client {
                ClientId = "shoppingcart-service",
                ClientName = "shoppingcart-service",
                Description = "The client that shoppingcart-api uses to call other services",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = {
                    "catalog-api"
                }
            },
            new Client {
                ClientId = "admin",
                ClientName = "admin",
                Description = "client for testing that represents admin user",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = {
                    "shoppingcart-api",
                    "catalog-api"
                },
                // ids6 handles sub explicitly as user in token validation for introspection and without prefix sees this and tries to lookup user
                // https://github.com/DuendeSoftware/IdentityServer/blob/b05f8fb3bb3abb9a6e83329537f3ac2b527ed5fd/src/IdentityServer/Validation/Default/TokenValidator.cs#L200
                ClientClaimsPrefix = "client_",
                Claims = {
                    new ClientClaim("sub", "15dc38a9-e9a0-4d44-8244-c7e28b20c558")
                }
            }
        };
}
