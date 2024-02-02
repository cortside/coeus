using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Events;
using Acme.IdentityServer.WebApi.Services;
using Acme.IdentityServer.WebApi.Services.ExtensionGrantValidators;
using Cortside.Common.BootStrap;
using IdentityServer4;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Acme.IdentityServer.WebApi.Installers {
    public class IdentityServerInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            var authConnString = configuration["Database:ConnectionString"];
            // Add a DbContext to store your Database Keys
            services.AddDbContext<DataProtectionKeyContext>(options =>
                options.UseSqlServer(authConnString)
            );

            services.AddDataProtection().PersistKeysToDbContext<DataProtectionKeyContext>();

            // need to register event service before calling AddIdentityServer
            services.AddTransient<IEventService, UserEnrichingEventService>();

            IIdentityServerBuilder idsBuilder = services.AddIdentityServer(options => {
                if (!string.IsNullOrWhiteSpace(configuration["issuerUri"])) {
                    options.IssuerUri = configuration["issuerUri"];
                }

                options.AccessTokenJwtType = "JWT";
                // this has to be set otherwise aud is not set
                options.EmitStaticAudienceClaim = true;

                // enable logging of events
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
            });

            if (!string.IsNullOrEmpty(configuration["IdentityServer:SigningCertificate"])) {
                var certificatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), configuration["IdentityServer:SigningCertificate"]);
                idsBuilder.AddSigningCredential(new X509Certificate2(certificatePath), configuration["IdentityServer:SigningCertificatePassword"]);
            } else {
                idsBuilder.AddDeveloperSigningCredential(false);
            }

            idsBuilder.AddConfigurationStore(options => {
                options.DefaultSchema = "auth";
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(authConnString);
            })
                    .AddOperationalStore(options => {
                        options.DefaultSchema = "auth";
                        options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(authConnString);

                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                        options.TokenCleanupInterval = 30;
                    })
                    .AddProfileService<UserProfileService>()
                    .AddCustomTokenRequestValidator<DelegationTokenRequestValidator>()
                    .AddExtensionGrantValidator<RecaptchaGrantValidator>()
                    .AddExtensionGrantValidator<DelegationGrantValidator>();

            var authenticationBuilder = services.AddAuthentication();

            var providers = configuration.GetSection("ExternalProviders").Get<List<Provider>>();
            services.AddSingleton(providers);

            foreach (var provider in providers) {
                authenticationBuilder.AddOpenIdConnect(provider.Scheme, provider.DisplayName, options => {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.Authority = provider.Authority;
                    options.ClientId = provider.ClientId;
                    options.CallbackPath = provider.CallbackPath;
                    foreach (var scope in provider.Scopes) {
                        options.Scope.Add(scope);
                    }
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = false
                    };
                    options.GetClaimsFromUserInfoEndpoint = true;
                    if (provider.AddIdTokenHint) {
                        options.SaveTokens = true;
                        options.Events.OnRedirectToIdentityProviderForSignOut += (Func<RedirectContext, Task>)(async context => {
                            context.ProtocolMessage.IdTokenHint = await context.HttpContext.GetTokenAsync(OpenIdConnectResponseType.IdToken);
                        });
                    }
                });
            }

            authenticationBuilder.AddIdentityServerAuthentication(options => {
                // configure authentication for non-openid endpoints
                options.SupportedTokens = SupportedTokens.Both;
                options.RequireHttpsMetadata = bool.Parse(configuration["IdentityServer:RequireHttpsMetadata"]);
                options.Authority = configuration["IdentityServer:Authority"];
                options.ApiName = configuration["IdentityServer:ApiName"];
                // for reference tokens to work correctly, secret needs to match with a SharedSecret in db
                // actual secret value in the db needs to be a matching base64 string of the SHA256/SHA512 hash, see HashedSharedSecretValidator.ValidateAsync() in ids source code
                options.ApiSecret = configuration["IdentityServer:ApiSecret"];
                //These 3 handlers were added so that TestServer in the Integration tests project would work
                options.JwtBackChannelHandler = Startup.Handler;

                options.ClaimsIssuer = configuration["IdentityServer:Authority"];
            });
        }
    }
}
