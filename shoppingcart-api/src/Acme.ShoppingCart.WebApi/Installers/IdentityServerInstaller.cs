using System.IdentityModel.Tokens.Jwt;
using Acme.ShoppingCart.Configuration;
using Cortside.Common.BootStrap;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.WebApi.Installers {
    public class IdentityServerInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var idsConfig = configuration.GetSection("IdentityServer").Get<IdentityServerConfiguration>();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options => {
                    // base-address of your identityserver
                    options.Authority = idsConfig.Authority;
                    options.RequireHttpsMetadata = idsConfig.RequireHttpsMetadata;
                    options.RoleClaimType = "role";
                    options.NameClaimType = "name";

                    // name of the API resource
                    options.ApiName = idsConfig.ApiName;
                    options.ApiSecret = idsConfig.ApiSecret;

                    options.EnableCaching = idsConfig.EnableCaching;
                    options.CacheDuration = idsConfig.CacheDuration;
                });

            // policy server
            configuration["PolicyServer:TokenClient:Authority"] = idsConfig.Authority;
            configuration["PolicyServer:TokenClient:ClientId"] = idsConfig.Authentication.ClientId;
            configuration["PolicyServer:TokenClient:ClientSecret"] = idsConfig.Authentication.ClientSecret;
            services.AddPolicyServerRuntimeClient(configuration.GetSection("PolicyServer"))
                .AddAuthorizationPermissionPolicies();
        }
    }
}
