#pragma warning disable CS1591 // Missing XML comments

using Acme.ShoppingCart.WebApi.Mappers;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.WebApi.Installers {
    public class ModelMapperInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            services.AddSingletonClassesBySuffix<OrderModelMapper>("Mapper");
        }
    }
}
