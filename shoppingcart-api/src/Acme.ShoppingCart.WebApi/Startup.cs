using System.IO.Compression;
using System.Linq;
using Acme.ShoppingCart.BootStrap;
using Acme.ShoppingCart.WebApi.Installers;
using Cortside.AspNetCore;
using Cortside.AspNetCore.ApplicationInsights;
using Cortside.AspNetCore.Auditable.Middleware;
using Cortside.AspNetCore.Filters;
using Cortside.Common.BootStrap;
using Cortside.Common.Correlation;
using Cortside.Common.Json;
using Cortside.Common.Messages.Filters;
using Cortside.Health.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Acme.ShoppingCart.WebApi {
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup {
        private readonly BootStrapper bootstrapper = null;

        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration) {
            bootstrapper = new DefaultApplicationBootStrapper();
            bootstrapper.AddInstaller(new IdentityServerInstaller());
            bootstrapper.AddInstaller(new NewtonsoftInstaller());
            bootstrapper.AddInstaller(new SubjectPrincipalInstaller());
            bootstrapper.AddInstaller(new SwaggerInstaller());
            bootstrapper.AddInstaller(new ModelMapperInstaller());
            Configuration = configuration;
        }

        /// <summary>
        /// Config
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configure Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services) {
            var serviceName = Configuration["Service:Name"];
            services.AddApplicationInsightsTelemetry(o => {
                o.InstrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"];
                o.EnableAdaptiveSampling = false;
                o.EnableActiveTelemetryConfigurationSetup = true;
            });
            services.AddCloudRoleNameInitializer(serviceName);

            services.AddResponseCaching();
            services.AddResponseCompression(options => {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.Configure<BrotliCompressionProviderOptions>(options => {
                options.Level = CompressionLevel.Optimal;
            });
            services.Configure<GzipCompressionProviderOptions>(options => {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddCors();

            services.AddControllers(options => {
                options.CacheProfiles.Add("Default", new CacheProfile {
                    Duration = 30,
                    Location = ResponseCacheLocation.Any
                });
                //https://stackoverflow.com/questions/55127637/globally-modelstate-validation-in-asp-net-core-mvc
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

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped(sp => {
                return sp.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(sp.GetRequiredService<IActionContextAccessor>().ActionContext);
            });

            // warm all the serivces up, can chain these together if needed
            services.AddStartupTask<WarmupServicesStartupTask>();

            // this is used in the warmup tasks
            services.AddSingleton(services);

            services.AddSingleton(Configuration);
            bootstrapper.InitIoCContainer(Configuration as IConfigurationRoot, services);
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

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.RoutePrefix = "swagger";
                options.ShowExtensions();
                options.ShowCommonExtensions();
                options.EnableValidator();
                options.EnableFilter();
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

                foreach (var description in provider.ApiVersionDescriptions) {
                    var version = description.GroupName.ToLowerInvariant();
                    options.SwaggerEndpoint($"/swagger/{version}/swagger.json", "Acme.ShoppingCart Api " + version.ToUpper());
                }
            });

            foreach (var groupName in provider.ApiVersionDescriptions.Select(x => x.GroupName)) {
                app.UseReDoc(c => {
                    var version = groupName.ToLowerInvariant();
                    c.DocumentTitle = $"Acme.ShoppingCart Api Documentation {groupName.ToUpperInvariant()}";
                    c.RoutePrefix = $"api-docs/{version}";
                    c.SpecUrl = $"/swagger/{version}/swagger.json";
                });
            }

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

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
    }
}
