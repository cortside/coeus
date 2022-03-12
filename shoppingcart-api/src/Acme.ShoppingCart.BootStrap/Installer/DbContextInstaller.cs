using Acme.ShoppingCart.Data;
using Cortside.Common.BootStrap;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class DbContextInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(configuration.GetSection("Database").GetValue<string>("ConnectionString")));

            services.AddScoped<IDatabaseContext>(provider => provider.GetService<DatabaseContext>());

            // Register the service and implementation for the database context
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<DatabaseContext>());

            // for DbContextCheck
            services.AddTransient<DbContext, DatabaseContext>();
        }
    }
}
