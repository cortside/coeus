using System.Linq;
using System.Reflection;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.DomainService.Mappers;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class DomainInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            // register all services, handlers and factories
            Assembly.GetEntryAssembly().GetTypes()
                .Where(x => (x.Name.EndsWith("Service") || x.Name.EndsWith("Handler") || x.Name.EndsWith("Factory") || x.Name.EndsWith("Client"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract
                    && x.GetInterfaces().Any())
                .ToList().ForEach(x => {
                    x.GetInterfaces().ToList()
                        .ForEach(i => services.AddScoped(i, x));
                });

            // register domain services
            typeof(OrderService).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Service"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract
                    && x.GetInterfaces().Any())
                .ToList().ForEach(x => {
                    x.GetInterfaces().ToList()
                        .ForEach(i => services.AddScoped(i, x));
                });

            typeof(OrderRepository).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Repository"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract
                    && x.GetInterfaces().Any())
                .ToList().ForEach(x => {
                    x.GetInterfaces().ToList()
                        .ForEach(i => services.AddScoped(i, x));
                });

            typeof(OrderMapper).GetTypeInfo().Assembly.GetTypes()
                .Where(x => (x.Name.EndsWith("Mapper"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract)
                .ToList().ForEach(x => {
                    services.AddScoped(x);
                });
        }
    }
}
