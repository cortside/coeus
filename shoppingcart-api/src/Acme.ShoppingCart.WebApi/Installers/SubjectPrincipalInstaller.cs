using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cortside.Common.BootStrap;
using Cortside.Common.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.WebApi.Installers {
    public class SubjectPrincipalInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddOptions();
            services.AddHttpContextAccessor();
            services.AddScoped<ISubjectPrincipal, SubjectPrincipal>((sp) => {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

                // when there is no httpcontext available, assume that subject is the "service" itself
                var claims = new List<Claim>() {
                    new Claim(JwtRegisteredClaimNames.Sub, Guid.Empty.ToString())
                };
                var system = new SubjectPrincipal(claims);

                if (httpContextAccessor?.HttpContext != null) {
                    var principal = new SubjectPrincipal(httpContextAccessor.HttpContext.User);
                    if (principal.SubjectId != null) {
                        return principal;
                    }
                }

                return system;
            });
        }
    }
}
