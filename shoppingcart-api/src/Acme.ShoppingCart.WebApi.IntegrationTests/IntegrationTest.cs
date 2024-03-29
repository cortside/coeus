using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Dynamic.Core;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.WebApi.IntegrationTests.Mocks;
using Cortside.MockServer.AccessControl;
using Cortside.MockServer.AccessControl.Models;
using Cortside.MockServer.Builder;
using Cortside.MockServer.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Acme.ShoppingCart.WebApi.IntegrationTests {
    public class IntegrationTest : WebApiIntegrationTest<Startup> {
        public IntegrationTest() {
            Subjects = JsonConvert.DeserializeObject<Subjects>(File.ReadAllText("./Data/subjects.json"));
        }

        protected override void ConfigureMockHttpServer(IMockHttpServerBuilder builder) {
            builder.AddMock<CommonMock>()
                .AddMock(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .AddMock(new SubjectMock("./Data/subjects.json"))
                .AddMock<CatalogMock>();
        }

        protected override void ConfigureConfiguration(IConfigurationBuilder builder) {
            builder.AddJsonFile("integrationsettings.json")
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
                    });
        }

        protected override void ConfigureServices(IServiceCollection services) {
            services.AddLogging(builder => builder.ClearProviders().AddConsole().AddDebug());

            var useInMemory = bool.Parse(Configuration["IntegrationTestFactory:InMemoryDatabase"] ?? "false");
            if (useInMemory) {
                services.RegisterInMemoryDbContext<DatabaseContext>(Api.TestId, db => {
                    try {
                        DatabaseFixture.SeedInMemoryDb(db);
                        if (!db.Subjects.Any()) {
                            throw new DbUpdateException();
                        }
                    } catch (Exception ex) {
                        throw new InvalidOperationException($"An error occurred seeding the database. Error: {ex.Message}", ex);
                    }
                });
                services.RegisterFileSystemDistributedLock();
            }

            services.RegisterDomainEventPublisher();
            SerializerSettings = services.ResolveSerializerSettings();
        }
    }
}
