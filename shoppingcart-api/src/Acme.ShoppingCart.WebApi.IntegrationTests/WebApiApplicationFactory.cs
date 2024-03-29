using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Net.Http.Headers;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.WebApi.IntegrationTests.Mocks;
using Cortside.AspNetCore.Builder;
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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.WebApi.IntegrationTests {
    public class WebApiApplicationFactory : WebApplicationFactory<Startup> {
        private bool disposed = false;
        private Subjects subjects;
        // testId is created outside of the options so that it's constant and not reevaluated at instance creation time
        private readonly string testId = Guid.NewGuid().ToString();

        public MockHttpServer MockServer { get; }
        public JsonSerializerSettings SerializerSettings { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public WebApiApplicationFactory() {
            MockServer = MockHttpServer.CreateBuilder(testId)
                .AddMock<CommonMock>()
                .AddMock(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .AddMock(new SubjectMock("./Data/subjects.json"))
                .AddMock<CatalogMock>()
                .Build();

            Configuration = GetConfiguration();
        }

        protected override IHost CreateHost(IHostBuilder builder) {
            // https://github.com/dotnet/aspnetcore/issues/37680#issuecomment-1331559463
            TestConfiguration.Create(b => b.AddConfiguration(Configuration));
            var host = base.CreateHost(builder);
            return host;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            builder.ConfigureAppConfiguration(config => {
                config.AddConfiguration(Configuration);
            });

            builder.ConfigureTestServices(services => {
                services.AddLogging(builder => builder.ClearProviders().AddConsole().AddDebug());

                var useInMemory = bool.Parse(Configuration["IntegrationTestFactory:InMemoryDatabase"]);
                if (useInMemory) {
                    RegisterInMemoryDbContext(services);
                    RegisterFileSystemDistributedLock(services);
                }

                RegisterDomainEventPublisher(services);
                ResolveSerializerSettings(services);
            });
        }

        private IConfiguration GetConfiguration() {
            var cfg = new ConfigurationBuilder()
                .AddJsonFile("integrationsettings.json", false, false)
                .AddInMemoryCollection(
                    new Dictionary<string, string> {
                        ["HealthCheckHostedService:Checks:1:Value"] = $"{MockServer.Url}/api/health",
                        ["HealthCheckHostedService:Checks:2:Value"] = $"{MockServer.Url}/api/health",
                        ["HealthCheckHostedService:Checks:4:Value"] = $"{MockServer.Url}/api/health",
                        ["IdentityServer:Authority"] = MockServer.Url,
                        ["IdentityServer:RequireHttpsMetadata"] = "false",
                        ["PolicyServer:TokenClient:Authority"] = MockServer.Url,
                        ["PolicyServer:PolicyServerUrl"] = MockServer.Url,
                        ["DistributedLock:UseRedisLockProvider"] = "false",
                        ["CatalogApi:ServiceUrl"] = $"{MockServer.Url}",
                        ["CatalogApi:Authentication:Url"] = $"{MockServer.Url}/connect/token"
                    })
                .Build();

            return cfg;
        }

        public RestApiClient CreateRestApiClient(ITestOutputHelper output) {
            var httpClient = CreateClient();
            return new RestApiClient(new XunitLogger("IntegrationTest", output), new HttpContextAccessor(), new RestApiClientOptions(), httpClient);
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
            services.RemoveAll<DbContextOptions<DatabaseContext>>();
            services.RemoveAll<DbContext>();

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
            services.RemoveAll<IDomainEventPublisher>();
            services.RemoveAll<IDomainEventReceiver>();

            services.AddDomainEventStubs();
        }

        // TODO: extension
        private void RegisterFileSystemDistributedLock(IServiceCollection services) {
            services.RemoveAll<IDistributedLockProvider>();

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

        public new void Dispose() {
            // Dispose of unmanaged resources.
            Dispose(true);
        }

        protected override void Dispose(bool disposing) {
            if (disposed) {
                return;
            }

            if (disposing) {
                MockServer?.Stop();
                MockServer?.Dispose();
            }

            disposed = true;
            base.Dispose(disposing);
        }
    }
}
