using Acme.ShoppingCart.BootStrap;
using Acme.ShoppingCart.WebApi.Filters;
using Acme.ShoppingCart.WebApi.Installers;
using Cortside.Common.BootStrap;
using Cortside.Common.Correlation;
using Cortside.Common.Json;
using Cortside.Common.Messages.Filters;
using Cortside.Health.Controllers;
using Microsoft.ApplicationInsights.Extensibility;
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
            bootstrapper.AddInstaller(new FacadeInstaller());
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
            services.AddSingleton<ITelemetryInitializer, AppInsightsInitializer>();
            services.AddApplicationInsightsTelemetry(o => {
                o.InstrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"];
                o.EnableAdaptiveSampling = false;
                o.EnableActiveTelemetryConfigurationSetup = true;
            });

            services.AddResponseCaching();
            services.AddResponseCompression(options => {
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddCors();

            services.AddControllers(options => {
                options.CacheProfiles.Add("default", new CacheProfile {
                    Duration = 30,
                    Location = ResponseCacheLocation.Any
                });
                //https://stackoverflow.com/questions/55127637/globally-modelstate-validation-in-asp-net-core-mvc
                options.Filters.Add<MessageExceptionResponseFilter>();
                options.Conventions.Add(new ApiControllerVersionConvention());
            })
            .AddNewtonsoftJson(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter {
                    DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"
                });
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

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.RoutePrefix = "swagger";
                //options.DefaultModelExpandDepth(-1);
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

            foreach (var description in provider.ApiVersionDescriptions) {
                app.UseReDoc(c => {
                    var version = description.GroupName.ToLowerInvariant();
                    c.DocumentTitle = $"MY API Documentation {description.GroupName.ToUpperInvariant()}";
                    c.RoutePrefix = $"api-docs/{version}";
                    c.SpecUrl = $"/swagger/{version}/swagger.json";
                });
            }

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseCors(builder => builder
                .WithOrigins(Configuration.GetSection("Cors").GetSection("Origins").Get<string[]>())
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
