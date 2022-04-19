using Cortside.AspNetCore.Auditable;
using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.WebApi.Installers {
    public class SubjectPrincipalInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddOptions();
            services.AddHttpContextAccessor();
            services.AddSubjectPrincipal();
        }
    }
}
