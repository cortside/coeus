using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.WebApi.IntegrationTests.Helpers;
using Acme.ShoppingCart.WebApi.IntegrationTests.Helpers.Mocks;
using Acme.ShoppingCart.WebApi.IntegrationTests.Helpers.Mocks.Models;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Stub;
using Medallion.Threading;
using Medallion.Threading.FileSystem;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace Acme.ShoppingCart.WebApi.IntegrationTests {
    public static class WireMockSetup {
        public static string WireMockUrl { get; set; }
        public static string RoutePrefix { get; set; }
        public static bool IsStarted { get; set; } = false;
        public static BaseWireMock Server { get; set; }
    }

    public class IntegrationTestFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class {
        private readonly string dbName = Guid.NewGuid().ToString();
        private Subjects subjects;
        public BaseWireMock WireMockServer { get; private set; }
        private IConfiguration Configuration { get; set; }

        protected override IHostBuilder CreateHostBuilder() {
            SetupConfiguration();
            SetupLogger();

            WireMockServer = new BaseWireMock(dbName)
                .ConfigureBuilder<CommonWireMock>()
                .ConfigureBuilder<IdsMock>()
                .ConfigureBuilder<SubjectMock>()
                .ConfigureBuilder<CatalogMock>();

            var section = Configuration.GetSection("HealthCheckHostedService");
            section["Checks:1:Value"] = $"{WireMockServer.WireMockUrl}/api/health";
            section["Checks:2:Value"] = $"{WireMockServer.WireMockUrl}/api/health";
            section["Checks:4:Value"] = $"{WireMockServer.WireMockUrl}/api/health";

            var authConfig = Configuration.GetSection("IdentityServer");
            authConfig["Authority"] = WireMockServer.WireMockUrl;
            authConfig["BaseUrl"] = $"{WireMockServer.WireMockUrl}/connect/token";
            authConfig["RequireHttpsMetadata"] = "false";

            var policyServerConfig = Configuration.GetSection("PolicyServer");
            var policyserverTokenClient = policyServerConfig.GetSection("TokenClient");
            policyserverTokenClient["Authority"] = WireMockServer.WireMockUrl;
            policyServerConfig["PolicyServerUrl"] = WireMockServer.WireMockUrl;

            var distributedLockConfig = Configuration.GetSection("DistributedLock");
            distributedLockConfig["UseRedisLockProvider"] = "false";

            var loanservicingConfig = Configuration.GetSection("LoanServicingApi");
            loanservicingConfig["BaseUrl"] = WireMockServer.WireMockUrl;
            var loanservicingAuthConfig = loanservicingConfig.GetSection("Authentication");
            loanservicingAuthConfig["Url"] = $"{WireMockServer.WireMockUrl}/connect/token";

            var userConfig = Configuration.GetSection("CatalogApi");
            userConfig["ServiceUrl"] = $"{WireMockServer.WireMockUrl}";
            var userAuthConfig = userConfig.GetSection("Authentication");
            userAuthConfig["Url"] = $"{WireMockServer.WireMockUrl}/connect/token";

            WireMockServer.WaitForStart();

            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(Configuration))
                .ConfigureWebHostDefaults(webbuilder => {
                    webbuilder
                    .UseConfiguration(Configuration)
                    .UseStartup<TStartup>()
                    .UseSerilog(Log.Logger)
                    .ConfigureTestServices(sc => {
                        var useInMemory = bool.Parse(Configuration["IntegrationTestFactory:InMemoryDatabase"]);
                        if (useInMemory) {
                            RegisterDbContext(sc);
                            RegisterFileSystemDistributedLock(sc);
                        }
                        RegisterDomainEventPublisher(sc);
                    });
                });
        }

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

        private void SetupConfiguration() {
            Configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.integration.json", optional: false, reloadOnChange: true)
                 .Build();
        }

        private void RegisterDbContext(IServiceCollection services) {
            // Remove the app's DbContext registration.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));
            if (descriptor != null) {
                services.Remove(descriptor);
            }
            descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContext));
            if (descriptor != null) {
                services.Remove(descriptor);
            }

            // register test instance
            services.AddDbContext<DatabaseContext>(options => {
                options.UseInMemoryDatabase(dbName);
                // enable sensisitive logging for debug logging, NEVER do this in production
                options.EnableSensitiveDataLogging();
                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            services.AddScoped<DbContext>(provider => provider.GetService<DatabaseContext>());

            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (DbContext).
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<DatabaseContext>();
            var logger = scopedServices.GetRequiredService<ILogger<IntegrationTestFactory<TStartup>>>();

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

        private void RegisterDomainEventPublisher(IServiceCollection services) {
            // Remove the real IDomainEventPublisher
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDomainEventPublisher));
            if (descriptor != null) {
                services.Remove(descriptor);
            }

            // Remove the real IDomainEventReceiver
            descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDomainEventReceiver));
            if (descriptor != null) {
                services.Remove(descriptor);
            }

            services.AddSingleton<IStubBroker, ConcurrentQueueBroker>();
            services.AddTransient<IDomainEventPublisher, DomainEventPublisherStub>();
            services.AddTransient<IDomainEventReceiver, DomainEventReceiverStub>();
        }

        private void RegisterFileSystemDistributedLock(IServiceCollection services) {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDistributedLockProvider));
            if (descriptor != null) {
                services.Remove(descriptor);
            }

            // install a file system one
            services.AddSingleton<IDistributedLockProvider>(_ => {
                return new FileDistributedSynchronizationProvider(new DirectoryInfo(Environment.CurrentDirectory));
            });
        }

        public HttpClient UnauthorizedClient {
            get {
                if (_UnauthorizedClient == null) {
                    _UnauthorizedClient = CreateDefaultClient();
                }
                return _UnauthorizedClient;
            }
        }

        private HttpClient _UnauthorizedClient { get; set; }

        public HttpClient CreateAuthorizedClient(string clientId) {
            var client = CreateDefaultClient();
            subjects ??= JsonConvert.DeserializeObject<Subjects>(File.ReadAllText("./Helpers/Mocks/subjects.json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", subjects.SubjectsList.First(s => s.ClientId == clientId).ReferenceToken);
            return client;
        }

        public HttpClient CreateCustomAuthorizedClient(Uri uri, string clientId) {
            var client = CreateDefaultClient(uri);
            subjects ??= JsonConvert.DeserializeObject<Subjects>(File.ReadAllText("./Helpers/Mocks/subjects.json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", subjects.SubjectsList.First(s => s.ClientId == clientId).ReferenceToken);
            return client;
        }

        public Subjects GetAllSubjects() {
            return subjects;
        }

        public Subject GetSubjectByName(string name) {
            return subjects.SubjectsList.First(x => x.ClientId == name);
        }

        internal DatabaseContext NewScopedDbContext() {
            var scope = Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        }
    }
}
