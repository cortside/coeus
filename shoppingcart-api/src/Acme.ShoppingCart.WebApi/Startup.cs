using Acme.DomainEvent.Events;
using Acme.ShoppingCart.BootStrap;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.DomainEvent;
using Acme.ShoppingCart.Health;
using Acme.ShoppingCart.WebApi.Installers;
using Cortside.AspNetCore;
using Cortside.AspNetCore.AccessControl;
using Cortside.AspNetCore.ApplicationInsights;
using Cortside.AspNetCore.Auditable;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.AspNetCore.Builder;
using Cortside.AspNetCore.EntityFramework;
using Cortside.AspNetCore.Swagger;
using Cortside.DomainEvent;
using Cortside.DomainEvent.EntityFramework;
using Cortside.Health;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Acme.ShoppingCart.WebApi {
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup : IWebApiStartup {
        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public Startup() {
        }

        /// <summary>
        /// Config
        /// </summary>
        private IConfiguration Configuration { get; set; }

        public void UseConfiguration(IConfiguration config) {
            Configuration = config;
        }

        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services) {
            // setup default json serializer settings
            JsonConvert.DefaultSettings = JsonNetUtility.GlobalDefaultSettings;

            // add ApplicationInsights telemetry
            var serviceName = Configuration["Service:Name"];
            var config = Configuration.GetSection("ApplicationInsights").Get<ApplicationInsightsServiceOptions>();
            services.AddApplicationInsights(serviceName, config);

            // add database context with interfaces
            services.AddDatabaseContext<IDatabaseContext, DatabaseContext>(Configuration);

            // add health services
            services.AddHealth(o => {
                o.UseConfiguration(Configuration);
                o.AddCustomCheck("example", typeof(ExampleCheck));
            });

            // add domain event receiver with handlers
            services.AddDomainEventReceiver(o => {
                o.UseConfiguration(Configuration);
                o.AddHandler<OrderStateChangedEvent, OrderStateChangedHandler>();
            });

            // add domain event publish with outbox
            services.AddDomainEventOutboxPublisher<DatabaseContext>(Configuration);

            // add controllers and all of the api defaults
            services.AddApiDefaults();

            // add SubjectPrincipal for auditing
            services.AddSubjectPrincipal();
            services.AddTransient<ISubjectFactory<Subject>, DefaultSubjectFactory>();

            // Add the access control using IdentityServer and PolicyServer
            services.AddAccessControl(Configuration);

            // Add swagger with versioning and OpenID Connect configuration using Newtonsoft
            services.AddSwagger(Configuration, "Acme.ShoppingCart API", "Acme.ShoppingCart API", new[] { "v1", "v2" });

            // add service for handling encryption of search parameters
            services.AddEncryptionService(Configuration["Encryption:Secret"]);

            // setup and register boostrapper and it's installers
            services.AddBootStrapper<DefaultApplicationBootStrapper>(Configuration, o => {
                o.AddInstaller(new ModelMapperInstaller());
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider) {
            app.UseMiniProfiler();
            app.UseApiDefaults(Configuration);
            app.UseSwagger("Acme.ShoppingCart Api", provider);

            // order of the following matters
            app.UseAuthentication();
            app.UseSubjectPrincipal(); // intentionally set after UseAuthentication
            app.UseRouting();
            app.UseAuthorization(); // intentionally set after UseRouting
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
