using System;
using Acme.ShoppingCart.Data;
using Cortside.Common.BootStrap;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class DbContextInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            services.AddDbContext<DatabaseContext>(opt => {
                opt.UseSqlServer(configuration.GetSection("Prequalification").GetValue<string>("ConnectionString"),
                    sqlServerOptionsAction: sqlOptions => {
                        sqlOptions.EnableRetryOnFailure(

                            // TODO: these values should come from config -- including enabling retry

                            maxRetryCount: 2,
                            maxRetryDelay: TimeSpan.FromSeconds(1),
                            errorNumbersToAdd: null);
                        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });
                opt.EnableServiceProviderCaching();
            });

            services.AddScoped<IDatabaseContext>(provider => provider.GetService<DatabaseContext>());

            // Register the service and implementation for the database context
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<DatabaseContext>());

            // for DbContextCheck
            services.AddTransient<DbContext, DatabaseContext>();
        }
    }
}
