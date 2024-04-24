using Acme.ShoppingCart.Facade;
using Acme.ShoppingCart.Facade.Mappers;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class FacadeInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            services.AddScopedInterfacesBySuffix<OrderFacade>("Facade");
            services.AddSingletonClassesBySuffix<OrderMapper>("Mapper");
        }
    }
}
