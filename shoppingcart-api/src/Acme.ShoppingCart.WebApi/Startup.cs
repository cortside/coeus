using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;
using Acme.ShoppingCart.BootStrap;
using Acme.ShoppingCart.WebApi.Installers;
using Cortside.AspNetCore;
using Cortside.AspNetCore.AccessControl;
using Cortside.AspNetCore.ApplicationInsights;
using Cortside.AspNetCore.Auditable;
using Cortside.AspNetCore.Auditable.Middleware;
using Cortside.AspNetCore.Builder;
using Cortside.AspNetCore.Filters;
using Cortside.AspNetCore.Swagger;
using Cortside.Common.Correlation;
using Cortside.Common.Json;
using Cortside.Common.Messages.Filters;
using Cortside.Health.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;

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
            var serviceName = Configuration["Service:Name"];
            var instrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"];
            services.AddApplicationInsights(serviceName, instrumentationKey);

            // add response compression using gzip and brotli compression
            services.AddDefaultResponseCompression(CompressionLevel.Optimal);

            // add SubjectPrincipal for auditing
            services.AddSubjectPrincipal();

            services.AddResponseCaching();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddCors();
            services.AddOptions();

            services.AddControllers(options => {
                options.CacheProfiles.Add("Default", new CacheProfile {
                    Duration = 30,
                    Location = ResponseCacheLocation.Any
                });
                options.Filters.Add<MessageExceptionResponseFilter>();
                options.Conventions.Add(new ApiControllerVersionConvention());
            })
            .AddNewtonsoftJson(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));

                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;

                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                options.SerializerSettings.Converters.Add(new IsoTimeSpanConverter());
            })
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(HealthController).Assembly));

            services.AddRouting(options => {
                options.LowercaseUrls = true;
            });

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

            // warm all the serivces up, can chain these together if needed
            services.AddStartupTask<WarmupServicesStartupTask>();

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
            app.UseMiddleware<CorrelationMiddleware>();

            app.UseResponseCompression();
            app.UseResponseCaching();

            app.UseSwagger("Acme.ShoppingCart Api", provider);

            app.UseCors(builder => builder
                .WithOrigins(Configuration.GetSection("Cors").GetSection("Origins").Get<string[]>())
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();

            // intentionally set after UseAuthentication
            app.UseMiddleware<SubjectMiddleware>();

            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        public void UseConfiguration(IConfiguration config) {
            Configuration = config;
        }
    }
}
