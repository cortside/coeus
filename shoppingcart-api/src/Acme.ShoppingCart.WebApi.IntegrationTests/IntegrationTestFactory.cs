using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.WebApi.IntegrationTests.Mocks;
using Cortside.AspNetCore.Builder;
using Cortside.Common.Testing.Extensions;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Stub;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.MockServer.AccessControl.Models;
using Cortside.MockServer.Mocks;
using Medallion.Threading;
using Medallion.Threading.FileSystem;
using Microsoft.AspNetCore.Hosting;
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

namespace Acme.ShoppingCart.WebApi.IntegrationTests {
    public class IntegrationTestFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class, IWebApiStartup {
        // dbname is created outside of the options so that it's constant and not reevaluated at instance creation time
        private readonly string dbName = Guid.NewGuid().ToString();
        private Subjects subjects;
        public MockHttpServer MockServer { get; private set; }
        private IConfiguration Configuration { get; set; }
        public JsonSerializerSettings SerializerSettings { get; private set; }

        //public IntegrationTestFactory(ITestOutputHelper outputHelper) {

        //}

        protected override IHostBuilder CreateHostBuilder() {
            SetupConfiguration();
            SetupLogger();

            // TODO: create logger for test output?
            // ITestOutputHelper
            // https://github.com/dotnet/runtime/tree/main/src/libraries/Microsoft.Extensions.Logging.Debug/src

            MockServer = MockHttpServer.CreateBuilder(dbName)
                .AddMock<CommonMock>()
                .AddMock(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .AddMock(new SubjectMock("./Data/subjects.json"))
                .AddMock<CatalogMock>()
                .Build();

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

            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => builder.AddConfiguration(Configuration))
                .ConfigureWebHostDefaults(webbuilder => {
                    webbuilder
                    .UseConfiguration(Configuration)
                    .UseStartup<TStartup>()
                    .ConfigureTestServices(sc => {
                        var useInMemory = bool.Parse(Configuration["IntegrationTestFactory:InMemoryDatabase"]);
                        if (useInMemory) {
                            RegisterDbContext(sc);
                            RegisterFileSystemDistributedLock(sc);
                        }
                        RegisterDomainEventPublisher(sc);
                        ResolveSerializerSettings(sc);
                    });
                })
                .UseSerilog(Log.Logger);
        }

        private void ResolveSerializerSettings(IServiceCollection services) {
            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (DbContext).
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var o = scopedServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>();
            SerializerSettings = o.Value.SerializerSettings;
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
                 .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                 .Build();
        }

        private void RegisterDbContext(IServiceCollection services) {
            // Remove the app's DbContext registration.
            services.Unregister<DbContextOptions<DatabaseContext>>();
            services.Unregister<DbContext>();

            // register test instance
            services.AddDbContext<DatabaseContext>(options => {
                options.UseInMemoryDatabase(dbName);
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
            services.Unregister<IDomainEventPublisher>();
            services.Unregister<IDomainEventReceiver>();

            services.AddDomainEventStubs();
        }

        private void RegisterFileSystemDistributedLock(IServiceCollection services) {
            services.Unregister<IDistributedLockProvider>();

            // install a file system one
            services.AddSingleton<IDistributedLockProvider>(_ => {
                return new FileDistributedSynchronizationProvider(new DirectoryInfo(Environment.CurrentDirectory));
            });
        }

        public HttpClient UnauthorizedClient {
            get {
                _UnauthorizedClient ??= CreateDefaultClient();
                return _UnauthorizedClient;
            }
        }

        private HttpClient _UnauthorizedClient { get; set; }

        public HttpClient CreateAuthorizedClient(string clientId) {
            var client = CreateDefaultClient();
            subjects ??= JsonConvert.DeserializeObject<Subjects>(File.ReadAllText("./Data/subjects.json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", subjects.SubjectsList.First(s => s.ClientId == clientId).ReferenceToken);
            return client;
        }

        public HttpClient CreateCustomAuthorizedClient(Uri uri, string clientId) {
            var client = CreateDefaultClient(uri);
            subjects ??= JsonConvert.DeserializeObject<Subjects>(File.ReadAllText("./Data/subjects.json"));
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
