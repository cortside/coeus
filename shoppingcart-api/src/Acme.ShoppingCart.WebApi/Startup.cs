using System.Collections.Generic;
using System.Reflection;
using Acme.ShoppingCart.BootStrap;
using Acme.ShoppingCart.Health;
using Acme.ShoppingCart.WebApi.Installers;
using Cortside.AspNetCore;
using Cortside.AspNetCore.AccessControl;
using Cortside.AspNetCore.ApplicationInsights;
using Cortside.AspNetCore.Auditable;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.AspNetCore.Builder;
using Cortside.AspNetCore.Swagger;
using Cortside.Health;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

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

        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services) {
            // add ApplicationInsights telemetry
            var serviceName = Configuration["Service:Name"];
            var config = Configuration.GetSection("ApplicationInsights").Get<ApplicationInsightsServiceOptions>();
            services.AddApplicationInsights(serviceName, config);

            // add health services
            services.AddHealth(o => {
                o.UseConfiguration(Configuration);
                o.AddCustomCheck("example", typeof(ExampleCheck));
            });

            // add controllers and all of the api defaults
            services.AddApiDefaults();

            // add SubjectPrincipal for auditing
            services.AddSubjectPrincipal();
            services.AddTransient<ISubjectFactory<Subject>, DefaultSubjectFactory>();

            // Add the access control using IdentityServer and PolicyServer
            services.AddAccessControl(Configuration);

            // Add swagger with versioning and OpenID Connect configuration using Newtonsoft
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var versions = new List<OpenApiInfo> {
                new OpenApiInfo {
                    Version = "v1",
                    Title = "Acme.ShoppingCart API",
                    Description = "Acme.ShoppingCart API",
                },
                new OpenApiInfo {
                    Version = "v2",
                    Title = "Acme.ShoppingCart API",
                    Description = "Acme.ShoppingCart API",
                }
            };
            services.AddSwagger(Configuration, xmlFile, versions);

            // setup and register boostrapper and it's installers
            services.AddBootStrapper<DefaultApplicationBootStrapper>(Configuration, o => {
                o.AddInstaller(new NewtonsoftInstaller());
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

        public void UseConfiguration(IConfiguration config) {
            Configuration = config;
        }
    }
}
