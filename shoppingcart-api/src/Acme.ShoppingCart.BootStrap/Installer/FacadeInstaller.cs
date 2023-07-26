using System.Linq;
using System.Reflection;
using Acme.ShoppingCart.Facade;
using Acme.ShoppingCart.Facade.Mappers;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class FacadeInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            typeof(OrderFacade).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Facade"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract)
                .ToList()
                .ForEach(x => {
                    x.GetInterfaces().ToList()
                        .ForEach(i => services.AddScoped(i, x));
                });

            typeof(OrderMapper).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Mapper"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract)
                .ToList()
                .ForEach(x => services.AddSingleton(x));
        }
    }
}
