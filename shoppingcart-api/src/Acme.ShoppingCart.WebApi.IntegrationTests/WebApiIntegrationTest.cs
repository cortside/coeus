using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Cortside.Common.Testing.Logging.Xunit;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl.Models;
using Cortside.MockServer.Builder;
using Cortside.RestApiClient;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.WebApi.IntegrationTests {
    public abstract class WebApiIntegrationTest<TEntryPoint> : IDisposable where TEntryPoint : class {
        private readonly WebApiFactory<TEntryPoint> api;
        private bool disposed;

        public WebApiIntegrationTest() {
            api = new WebApiFactory<TEntryPoint>(
                configureConfiguration: builder => { ConfigureConfiguration(builder); },
                configureServices: services => { ConfigureServices(services); },
                configureMockHttpServer: builder => { ConfigureMockHttpServer(builder); }
            );
        }

        protected virtual void ConfigureConfiguration(IConfigurationBuilder builder) {
        }

        protected virtual void ConfigureServices(IServiceCollection services) {
        }

        protected virtual void ConfigureMockHttpServer(IMockHttpServerBuilder builder) {
        }

        public WebApiFactory<TEntryPoint> Api => api;

        public JsonSerializerSettings SerializerSettings { get; protected set; }

        public ITestOutputHelper TestOutputHelper {
            get { return api.TestOutputHelper; }
            set { api.TestOutputHelper = value; }
        }

        public MockHttpServer MockServer => api.MockServer;

        public IConfiguration Configuration => api.Configuration;

        public IServiceProvider Services => api.Services;

        public Subjects Subjects { get; protected set; }

        public RestApiClient CreateRestApiClient(ITestOutputHelper output) {
            var httpClient = api.CreateClient();
            var logger = new XunitLogger("IntegrationTest", output);
            return new RestApiClient(logger, new HttpContextAccessor(), new RestApiClientOptions(), httpClient);
        }

        public HttpClient UnauthorizedClient {
            get {
                SingleUnauthorizedClient ??= api.CreateDefaultClient();
                return SingleUnauthorizedClient;
            }
        }

        private HttpClient SingleUnauthorizedClient { get; set; }

        public HttpClient CreateAuthorizedClient(string clientId) {
            if (Subjects == null || Subjects.SubjectsList.Count == 0) {
                throw new InvalidOperationException("Subjects must be populated before calling this method");
            }

            var client = api.CreateDefaultClient();
            var token = Subjects.SubjectsList.First(s => s.ClientId == clientId).ReferenceToken;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public HttpClient CreateCustomAuthorizedClient(Uri uri, string clientId) {
            if (Subjects == null || Subjects.SubjectsList.Count == 0) {
                throw new InvalidOperationException("Subjects must be populated before calling this method");
            }

            var client = api.CreateDefaultClient(uri);
            var token = Subjects.SubjectsList.First(s => s.ClientId == clientId).ReferenceToken;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public TDbContext NewScopedDbContext<TDbContext>() where TDbContext : DbContext {
            var scope = api.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<TDbContext>();
        }

        public new void Dispose() {
            // Dispose of unmanaged resources.
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposed) {
                return;
            }

            if (disposing) {
                api?.Dispose();
            }

            disposed = true;
        }
    }
}
