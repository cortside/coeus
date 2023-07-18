using Cortside.Common.BootStrap;
using Medallion.Threading;
using Medallion.Threading.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class DistributedLockInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            var connectionString = configuration.GetSection("Database").GetValue<string>("ConnectionString");
            IDistributedLockProvider provider = new SqlDistributedSynchronizationProvider(connectionString);
            services.AddSingleton(provider);
        }
    }
}
