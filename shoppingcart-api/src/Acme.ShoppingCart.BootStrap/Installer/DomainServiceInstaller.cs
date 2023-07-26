using System.Linq;
using System.Reflection;
using Acme.ShoppingCart.DomainService;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class DomainServiceInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            // register domain services
            typeof(OrderService).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Service"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract
                    && x.GetInterfaces().Length > 0)
                .ToList().ForEach(x => {
                    x.GetInterfaces().ToList()
                        .ForEach(i => services.AddScoped(i, x));
                });
        }
    }
}
