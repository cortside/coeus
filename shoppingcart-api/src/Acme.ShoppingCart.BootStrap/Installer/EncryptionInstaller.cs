using Acme.ShoppingCart.Configuration;
using Acme.ShoppingCart.DomainService;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class EncryptionInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            var config = configuration.GetSection("Encryption").Get<EncryptionConfiguration>();
            services.AddSingleton(config);

            services.AddSingleton<IEncryptionService, EncryptionService>();
        }
    }
}
