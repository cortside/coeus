using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.WebApi.IntegrationTests.Mocks;
using Cortside.Common.Testing.Extensions;
using Cortside.Common.Testing.Logging.Xunit;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Stub;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.MockServer.AccessControl.Models;
using Cortside.MockServer.Mocks;
using Cortside.RestApiClient;
using Medallion.Threading;
using Medallion.Threading.FileSystem;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.WebApi.IntegrationTests {
    public class WebApiApplicationFactory : WebApplicationFactory<Startup>, IDisposable {
        private bool disposed = false;

        // testId is created outside of the options so that it's constant and not reevaluated at instance creation time
        private readonly string testId = Guid.NewGuid().ToString();
        private Subjects subjects;
        public MockHttpServer MockServer { get; private set; }
        public JsonSerializerSettings SerializerSettings { get; private set; }

        public IConfiguration Configuration { get; private set; }

        public WebApiApplicationFactory() {
            SetupConfiguration();
            SetupLogger();

            MockServer = MockHttpServer.CreateBuilder(testId)
                .AddMock<CommonMock>()
                .AddMock(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .AddMock(new SubjectMock("./Data/subjects.json"))
                .AddMock<CatalogMock>()
                .Build();
        }

        // TODO: extension
        private void SetupLogger() {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext();

            var serverUrl = Configuration["Seq:ServerUrl"];
            if (!string.IsNullOrWhiteSpace(serverUrl)) {
                loggerConfiguration.WriteTo.Seq(serverUrl);
            }

            var logFile = Configuration["LogFile:Path"];
            if (!string.IsNullOrWhiteSpace(logFile)) {
                loggerConfiguration.WriteTo.File(logFile);
            }
            Log.Logger = loggerConfiguration.CreateLogger();
        }

        // TODO: extension
        private void SetupConfiguration() {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.integration.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            builder.ConfigureAppConfiguration(config => {
                Configuration["HealthCheckHostedService:Checks:1:Value"] = $"{MockServer.Url}/api/health";
                Configuration["HealthCheckHostedService:Checks:2:Value"] = $"{MockServer.Url}/api/health";
                Configuration["HealthCheckHostedService:Checks:4:Value"] = $"{MockServer.Url}/api/health";
                Configuration["IdentityServer:Authority"] = MockServer.Url;
                Configuration["IdentityServer:RequireHttpsMetadata"] = "false";
                Configuration["PolicyServer:TokenClient:Authority"] = MockServer.Url;
                Configuration["PolicyServer:PolicyServerUrl"] = MockServer.Url;
                Configuration["DistributedLock:UseRedisLockProvider"] = "false";
                Configuration["CatalogApi:ServiceUrl"] = $"{MockServer.Url}";
                Configuration["CatalogApi:Authentication:Url"] = $"{MockServer.Url}/connect/token";

                // make sure background services are enabled for tests
                Configuration["HealthCheckHostedService:Enabled"] = "true";
                Configuration["ReceiverHostedService:Enabled"] = "true";

                config.AddConfiguration(Configuration);
            });

            builder.ConfigureTestServices(services => {
                var useInMemory = bool.Parse(Configuration["IntegrationTestFactory:InMemoryDatabase"]);
                if (useInMemory) {
                    RegisterInMemoryDbContext(services);
                    RegisterFileSystemDistributedLock(services);
                }

                RegisterDomainEventPublisher(services);
                ResolveSerializerSettings(services);
            });

            builder.ConfigureLogging(x => {
                x.AddSerilog(Log.Logger);
            });
        }

        // TODO: this should be an extension method in testing project
        public RestApiClient CreateRestApiClient(ITestOutputHelper output) {
            var httpClient = CreateClient();
            return new RestApiClient(new XunitLogger("SettingsTest", output), new HttpContextAccessor(), new RestApiClientOptions(), httpClient);
        }

        // TODO: extension
        private void ResolveSerializerSettings(IServiceCollection services) {
            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (DbContext).
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var o = scopedServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>();
            SerializerSettings = o.Value.SerializerSettings;
        }

        // TODO: extension for part -- rest in DatabaseFixture??
        private void RegisterInMemoryDbContext(IServiceCollection services) {
            // Remove the app's DbContext registration.
            services.Unregister<DbContextOptions<DatabaseContext>>();
            services.Unregister<DbContext>();

            // register test instance
            services.AddDbContext<DatabaseContext>(options => {
                options.UseInMemoryDatabase(testId);
                // enable sensitive logging for debug logging, NEVER do this in production
                options.EnableSensitiveDataLogging();
                // since in-memory database does not support transactions, ignore the warnings
                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            services.AddScoped<DbContext>(provider => provider.GetService<DatabaseContext>());

            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (DbContext).
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<DatabaseContext>();
            var logger = scopedServices.GetRequiredService<ILogger<WebApiApplicationFactory>>();

            // Ensure the database is created.
            db.Database.EnsureCreated();

            try {
                DatabaseFixture.SeedInMemoryDb(db);
                if (!db.Subjects.Any()) {
                    throw new DbUpdateException();
                }
            } catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
            }
        }

        // TODO: extension
        private void RegisterDomainEventPublisher(IServiceCollection services) {
            services.Unregister<IDomainEventPublisher>();
            services.Unregister<IDomainEventReceiver>();

            services.AddDomainEventStubs();
        }

        // TODO: extension
        private void RegisterFileSystemDistributedLock(IServiceCollection services) {
            services.Unregister<IDistributedLockProvider>();

            // install a file system one
            services.AddSingleton<IDistributedLockProvider>(_ => {
                return new FileDistributedSynchronizationProvider(new DirectoryInfo(Environment.CurrentDirectory));
            });
        }

        public void Dispose() {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposed) {
                return;
            }

            if (disposing) {
                MockServer?.Stop();
                MockServer?.Dispose();
            }

            disposed = true;
        }
    }
}
