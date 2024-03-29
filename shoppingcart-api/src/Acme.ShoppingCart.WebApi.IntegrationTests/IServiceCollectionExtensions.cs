using System;
using System.IO;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Stub;
using Medallion.Threading;
using Medallion.Threading.FileSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Acme.ShoppingCart.WebApi.IntegrationTests {
    public static class IServiceCollectionExtensions {
        public static JsonSerializerSettings ResolveSerializerSettings(this IServiceCollection services) {
            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (DbContext).
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var o = scopedServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>();
            return o.Value.SerializerSettings;
        }

        public static void RegisterInMemoryDbContext<TDatabaseContext>(this IServiceCollection services, string testId, Action<TDatabaseContext> seedDatabase) where TDatabaseContext : DbContext {
            // Remove the app's DbContext registration.
            services.RemoveAll<DbContextOptions<TDatabaseContext>>();
            services.RemoveAll<DbContext>();

            // register test instance
            services.AddDbContext<TDatabaseContext>(options => {
                options.UseInMemoryDatabase(testId);
                // enable sensitive logging for debug logging, NEVER do this in production
                options.EnableSensitiveDataLogging();
                // since in-memory database does not support transactions, ignore the warnings
                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            services.AddScoped<DbContext>(provider => provider.GetService<TDatabaseContext>());

            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (DbContext).
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<TDatabaseContext>();
            //var logger = scopedServices.GetRequiredService<ILogger<WebApiFactory<Startup>>>();

            // Ensure the database is created.
            db.Database.EnsureCreated();
            seedDatabase.Invoke(db);
        }

        public static void RegisterDomainEventPublisher(this IServiceCollection services) {
            services.RemoveAll<IDomainEventPublisher>()
                .RemoveAll<IDomainEventReceiver>()
                .AddDomainEventStubs();
        }

        public static void RegisterFileSystemDistributedLock(this IServiceCollection services) {
            services.RemoveAll<IDistributedLockProvider>();

            // install a file system one
            services.AddSingleton<IDistributedLockProvider>(_ => {
                return new FileDistributedSynchronizationProvider(new DirectoryInfo(Environment.CurrentDirectory));
            });
        }
    }
}
