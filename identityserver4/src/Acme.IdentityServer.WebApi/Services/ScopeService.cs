using System;
using System.Collections.Generic;
using System.Linq;
using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.IdentityServer.WebApi.Services {
    public class ScopeService : IScopeService {

        private readonly IServiceProvider serviceProvider;

        public ScopeService(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
        }

        public List<string> GetAll() {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var scopes = dbContext.ApiScopes.Where(c => c.Enabled);
                if (scopes.Count() > 0) {
                    return scopes.Select(x => x.Name).ToList();
                }
                return new List<string>();
            }
        }
    }
}
