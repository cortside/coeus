using System.Linq;
using System.Reflection;
using Acme.ShoppingCart.Data.Repositories;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class RepositoryInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            // register repositories
            typeof(OrderRepository).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Repository"))
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
