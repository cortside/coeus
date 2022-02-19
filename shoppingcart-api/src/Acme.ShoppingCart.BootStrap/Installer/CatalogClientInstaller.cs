using Acme.ShoppingCart.Configuration;
using Acme.ShoppingCart.UserClient;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class CatalogClientInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddScoped<ICatalogClient, CatalogClient>();
            var catalogClientConfiguration = configuration.GetSection("CatalogApi").Get<CatalogClientConfiguration>();

            var idsConfig = configuration.GetSection("IdentityServer").Get<IdentityServerConfiguration>();

            catalogClientConfiguration.Authentication = idsConfig.Authentication;
            catalogClientConfiguration.Authentication.AuthorityUrl = idsConfig.Authority;

            services.AddSingleton(catalogClientConfiguration);
        }
    }
}
