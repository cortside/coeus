using System.Linq;
using System.Reflection;
using Acme.ShoppingCart.WebApi.Facades;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.WebApi.Installers {
    public class FacadeInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            typeof(OrderFacade).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Facade"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract)
                .ToList().ForEach(x => {
                    x.GetInterfaces().ToList()
                        .ForEach(i => services.AddScoped(i, x));
                });
        }
    }
}
