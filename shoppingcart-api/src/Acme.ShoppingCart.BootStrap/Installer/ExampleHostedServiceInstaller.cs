using Acme.ShoppingCart.Configuration;
using Acme.ShoppingCart.Hosting;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class ExampleHostedServiceInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddSingleton(configuration.GetSection("ExampleHostedService").Get<ExampleHostedServiceConfiguration>());
            services.AddHostedService<ExampleHostedService>();
        }
    }
}
