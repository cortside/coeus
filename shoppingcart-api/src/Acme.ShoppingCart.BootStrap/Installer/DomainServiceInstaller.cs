using Acme.ShoppingCart.DomainService;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class DomainServiceInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            services.AddScopedInterfacesBySuffix<OrderService>("Service");
        }
    }
}
