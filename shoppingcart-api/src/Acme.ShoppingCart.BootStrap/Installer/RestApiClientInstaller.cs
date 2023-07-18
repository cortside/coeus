using Acme.ShoppingCart.CatalogApi;
using Acme.ShoppingCart.Configuration;
using Cortside.AspNetCore;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class RestApiClientInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            // register clients
            services.AddRestApiClient<ICatalogClient, CatalogClient, CatalogClientConfiguration>(configuration, "CatalogApi");

            var catalogClientConfiguration = configuration.GetSection("CatalogApi").Get<CatalogClientConfiguration>();

            var idsConfig = configuration.GetSection("IdentityServer").Get<IdentityServerConfiguration>();

            catalogClientConfiguration.Authentication = idsConfig.Authentication;
            catalogClientConfiguration.Authentication.AuthorityUrl = idsConfig.Authority;

            services.AddSingleton(catalogClientConfiguration);
        }
    }
}
