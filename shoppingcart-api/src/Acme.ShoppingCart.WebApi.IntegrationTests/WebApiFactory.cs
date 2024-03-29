using System;
using Cortside.AspNetCore.Builder;
using Cortside.MockServer;
using Cortside.MockServer.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.WebApi.IntegrationTests {
    public class WebApiFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class {
        private bool disposed;
        // testId is created outside of the options so that it's constant and not reevaluated at instance creation time
        private readonly string testId = Guid.NewGuid().ToString();

        private readonly Action<IServiceCollection> configureServices;
        private readonly Action<IConfigurationBuilder> configureConfiguration;
        private readonly Action<IMockHttpServerBuilder> configureMockHttpServer;

        public WebApiFactory(Action<IMockHttpServerBuilder> configureMockHttpServer = null, Action<IConfigurationBuilder> configureConfiguration = null, Action<IServiceCollection> configureServices = null) {
            this.configureServices = configureServices;
            this.configureConfiguration = configureConfiguration;
            this.configureMockHttpServer = configureMockHttpServer;
        }

        public ITestOutputHelper TestOutputHelper { get; set; }
        public MockHttpServer MockServer { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public string TestId => testId;

        protected override IHost CreateHost(IHostBuilder builder) {
            // configure and start the mock http server, needed before configuration can be set
            var mb = MockHttpServer.CreateBuilder(testId);
            configureMockHttpServer.Invoke(mb);
            MockServer = mb.Build();

            // configure....configuration
            var cb = new ConfigurationBuilder();
            configureConfiguration.Invoke(cb);
            Configuration = cb.Build();

            // set integration configuration so that it's available for webapplication
            // https://github.com/dotnet/aspnetcore/issues/37680#issuecomment-1331559463
            TestConfiguration.Create(b => b.AddConfiguration(Configuration));

            // build and return the host
            var host = base.CreateHost(builder);
            return host;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            builder.ConfigureAppConfiguration(config => {
                config.AddConfiguration(Configuration);
            });

            builder.ConfigureTestServices(services => {
                configureServices.Invoke(services);
            });
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
