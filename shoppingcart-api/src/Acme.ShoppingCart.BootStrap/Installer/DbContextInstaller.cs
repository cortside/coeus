using Acme.ShoppingCart.Data;
using Cortside.AspNetCore.EntityFramework;
using Cortside.Common.BootStrap;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class DbContextInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddDbContext<DatabaseContext>(opt => {
                opt.UseSqlServer(configuration.GetSection("Database").GetValue<string>("ConnectionString"),
                    sqlServerOptionsAction: sqlOptions => {
                        // can not use EnableRetryOnFailure because of the use of user initiated transactions

                        // instruct ef to use multiple queries instead of large joined queries
                        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });
                opt.EnableServiceProviderCaching();
            });

            // register the dbcontext interface that is to be used
            services.AddScoped<IDatabaseContext>(provider => provider.GetService<DatabaseContext>());

            // Register the service and implementation for the database context
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<DatabaseContext>());

            // for DbContextCheck in cortside.health
            services.AddTransient<DbContext, DatabaseContext>();
        }
    }
}
