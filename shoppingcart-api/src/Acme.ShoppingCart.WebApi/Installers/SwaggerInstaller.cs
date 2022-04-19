using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Acme.ShoppingCart.Configuration;
using Cortside.AspNetCore.Swagger.Filters;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Acme.ShoppingCart.WebApi.Installers {
    public class SwaggerInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddApiVersioning(o => {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = false;
                o.UseApiBehavior = true;
            });

            services.AddVersionedApiExplorer(options => {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = false;
            });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Version = "v1",
                    Title = "Acme.ShoppingCart API",
                    Description = "Acme.ShoppingCart API",
                });

                c.SwaggerDoc("v2", new OpenApiInfo {
                    Version = "v2",
                    Title = "Acme.ShoppingCart API",
                    Description = "Acme.ShoppingCart API",
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.OperationFilter<RemoveVersionFromParameter>();
                c.OperationFilter<AuthorizeOperationFilter>();
                c.DocumentFilter<ReplaceVersionWithExactValueInPath>();
                c.IgnoreObsoleteActions();
                c.TagActionsBy(c => new[] { c.RelativePath });

                // https://www.scottbrady91.com/identity-server/aspnet-core-swagger-ui-authorization-using-identityserver4
                var idsConfig = configuration.GetSection("IdentityServer").Get<IdentityServerConfiguration>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows {
                        ClientCredentials = new OpenApiOAuthFlow {
                            AuthorizationUrl = new Uri($"{idsConfig.Authority}/connect/authorize"),
                            TokenUrl = new Uri($"{idsConfig.Authority}/connect/token"),
                            Scopes = new Dictionary<string, string> {
                                {idsConfig.ApiName, idsConfig.ApiName}
                            }
                        }
                    }
                });
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }
    }
}
