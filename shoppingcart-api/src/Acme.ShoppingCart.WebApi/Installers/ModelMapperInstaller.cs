using System.Linq;
using System.Reflection;
using Acme.ShoppingCart.WebApi.Mappers;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.WebApi.Installers {
    public class ModelMapperInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            typeof(OrderModelMapper).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Mapper"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract)
                .ToList().ForEach(x => {
                    services.AddScoped(x);
                });
        }
    }
}
