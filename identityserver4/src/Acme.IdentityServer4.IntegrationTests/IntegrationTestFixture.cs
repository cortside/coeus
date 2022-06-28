using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Cortside.DomainEvent;
using Cortside.IdentityServer.WebApi.IntegrationTests.Helpers;
using Cortside.IdentityServer.Data;
using Cortside.IdentityServer.WebApi.Security;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog;

namespace Cortside.IdentityServer.WebApi.IntegrationTests {
    public class IntegrationTestFixture : IDisposable {
        private readonly TestServer testserver;
        public IIdentityServerDbContext DbContext;
        public HttpClient Client { get; set; }
        public HttpClient DiscoClient { get; set; }
        public Mock<ISubjectPrincipal> subjectMock;
        public Mock<IHttpContextAccessor> httpMock;
        public readonly string ClientName = "testClient";
        public readonly string ClientSecret = Convert.ToBase64String(Encoding.ASCII.GetBytes("secret"));
        public readonly string Scope = "identity";
        public readonly string Password = "moreStuff";
        public readonly string UserName = "Biff";
        public Guid CreateUserId = Guid.NewGuid();
        public Guid DefaultUserId = Guid.NewGuid();

        public IntegrationTestFixture() {
            subjectMock = new Mock<ISubjectPrincipal>();
            subjectMock.Setup(x => x.SubjectId).Returns(CreateUserId);
            httpMock = new Mock<IHttpContextAccessor>();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();
            DomainEventPublisherStub publisherStub = new DomainEventPublisherStub();
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext();
            var serverUrl = configuration["Seq:ServerUrl"];
            if (!String.IsNullOrWhiteSpace(serverUrl)) {
                loggerConfiguration.WriteTo.Seq(serverUrl);
            }
            Log.Logger = loggerConfiguration.CreateLogger();
            IWebHostBuilder builder = new WebHostBuilder()
                .UseSerilog(Log.Logger)
                .UseConfiguration(configuration)
                .UseStartup<Cortside.IdentityServer.WebApi.Startup>()
                .ConfigureTestServices(sc => {

                    sc.AddSingleton<IDomainEventPublisher>(publisherStub);
                    sc.AddSingleton<ISubjectPrincipal>(subjectMock.Object);
                    RegisterDbContext(sc);
                    sc.AddSingleton<IDomainEventOutboxPublisher>(publisherStub);
                });
            testserver = new TestServer(builder);
            Cortside.IdentityServer.WebApi.Startup.Handler = testserver.CreateHandler();
            Client = testserver.CreateClient();
        }

        private void RegisterDbContext(IServiceCollection sc) {
            var dbName = $"DBNAME_{Guid.NewGuid()}";
            var dbOptions = new DbContextOptionsBuilder<IdentityServerDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            IdentityServerDbContext context = new IdentityServerDbContext(dbOptions, subjectMock.Object, httpMock.Object);
            IIdentityServerDbContext dbContext = context;
            SeedInMemoryLoanDb(dbContext);
            sc.AddSingleton(context);
            sc.AddSingleton(dbContext);

            sc.AddIdentityServerBuilder()
                .AddConfigurationStore(options => {
                    options.ConfigureDbContext = builder =>
                        builder.UseInMemoryDatabase(dbName);
                })
                .AddOperationalStore(options => {
                    options.ConfigureDbContext = builder =>
                        builder.UseInMemoryDatabase(dbName);
                })
                .AddInMemoryClients(new List<Client> {
                    GetDefaultClient(),
                    GetDefaultClient("roadrunnercorp", "eboa.webapi", null, "7fd2e357-f41d-4e41-95db-85d7b4f8518e"),
                    GetDefaultClient("acme", "eboa.webapi", null, "36c90fc9-496c-40cd-87d9-23dc3f79ae2c"),
                    GetDefaultClient("application-api", "document-api", null, "d2e9c5de-f619-4a64-b657-32f8abc5ebb6"),
                })
                .AddInMemoryApiScopes(new List<ApiScope> {
                    GetDefaultApiScope(),
                    GetDefaultApiScope("eboa.webapi"),
                    GetDefaultApiScope("document-api"),
                })
                .AddInMemoryApiResources(new List<ApiResource> {
                    GetDefaultApiResource(),
                    GetDefaultApiResource("eboa.webapi"),
                    GetDefaultApiResource("document-api"),
                    GetDefaultApiResource("application-api"),
                });
        }

        private ApiResource GetDefaultApiResource(string scope = null) {
            scope ??= Scope;
            return new ApiResource {
                Name = scope,
                Description = scope,
                DisplayName = scope,
                Scopes = new List<string> { scope },
                Enabled = true,
                UserClaims = new List<string> { "role" },
                ApiSecrets = new List<Secret> { GetDefaultSecret() },
                Properties = new Dictionary<string, string>(),
            };
        }

        private Client GetDefaultClient(string clientName = null, string scopeName = null, string secret = null, string sub = null) {
            sub ??= Guid.NewGuid().ToString();
            clientName ??= ClientName;
            scopeName ??= Scope;
            return new Client() {
                AbsoluteRefreshTokenLifetime = 2592000,
                AccessTokenType = 0,
                AccessTokenLifetime = 3600,
                AllowAccessTokensViaBrowser = true,
                AllowOfflineAccess = false,
                AllowPlainTextPkce = false,
                AllowRememberConsent = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AlwaysSendClientClaims = true,
                AuthorizationCodeLifetime = 300,
                BackChannelLogoutSessionRequired = true,
                ClientClaimsPrefix = string.Empty,
                ClientId = clientName,
                ClientName = clientName,
                EnableLocalLogin = true,
                Enabled = true,
                FrontChannelLogoutSessionRequired = true,
                IdentityTokenLifetime = 300,
                IncludeJwtId = false,
                ProtocolType = "oidc",
                RequireClientSecret = true,
                RequireConsent = false,
                RequirePkce = false,
                SlidingRefreshTokenLifetime = 1296000,
                UpdateAccessTokenClaimsOnRefresh = false,
                AllowedGrantTypes = new List<string> { "client_credentials", "delegation" },
                ClientSecrets = new List<Secret> {
                    GetDefaultSecret(secret)
                },
                AllowedScopes = new List<string> { GetDefaultApiScope(scopeName).Name },
                Claims = new List<ClientClaim> {
                    new ClientClaim { Type = "sub", Value = sub },
                    new ClientClaim { Type = "email", Value = "wile_e_coyote@acme.com" },
                }
            };
        }

        private Secret GetDefaultSecret(string secret = null) {
            secret ??= ClientSecret;
            return new Secret {
                Type = "SharedSecret",
                Value = secret.Sha512(),
            };
        }

        private ApiScope GetDefaultApiScope(string name = null) {
            name ??= Scope;
            return new ApiScope {
                DisplayName = name,
                Enabled = true,
                Description = name,
                Name = name,
                Emphasize = false,
                Required = true
            };
        }

        private void SeedInMemoryLoanDb(IIdentityServerDbContext dbContext) {
            dbContext.AddUser(new User {
                CreateUserId = CreateUserId,
                Password = Password,
                UserId = DefaultUserId,
                Username = UserName,
            });
            dbContext.SaveChanges();
        }

        public void Dispose() {
            testserver.Dispose();
            Client.Dispose();
        }
    }
}
