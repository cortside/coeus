using Acme.ShoppingCart.Data.Repositories;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class RepositoryInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            services.AddScopedInterfacesBySuffix<OrderRepository>("Repository");
        }
    }
}
