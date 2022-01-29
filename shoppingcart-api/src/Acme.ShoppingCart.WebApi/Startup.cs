using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using Acme.ShoppingCart.BootStrap;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.UserClient;
using Cortside.Common.BootStrap;
using Cortside.Common.Correlation;
using Cortside.Common.Json;
using Cortside.Common.Messages.Filters;
using Cortside.Common.Security;
using Cortside.DomainEvent.EntityFramework.Hosting;
using Cortside.Health.Controllers;
using IdentityServer4.AccessTokenValidation;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Formatters;
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

            IsoDateTimeConverter isoConverter = new IsoDateTimeConverter {
                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"
            };

            IsoTimeSpanConverter isoTimeSpanConverter = new IsoTimeSpanConverter();

            JsonConvert.DefaultSettings = () => {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                settings.Converters.Add(isoConverter);
                settings.Converters.Add(isoTimeSpanConverter);
                return settings;
            };

            services.AddControllers(options => {
                options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                options.CacheProfiles.Add("default", new CacheProfile {
                    Duration = 30,
                    Location = ResponseCacheLocation.Any
                });
                //https://stackoverflow.com/questions/55127637/globally-modelstate-validation-in-asp-net-core-mvc
                options.Filters.Add<MessageExceptionResponseFilter>();
            })
            .AddNewtonsoftJson(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                options.SerializerSettings.Converters.Add(isoConverter);
                options.SerializerSettings.Converters.Add(isoTimeSpanConverter);
            })
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(HealthController).Assembly));

            services.AddRouting(options => {
                options.LowercaseUrls = true;
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped(sp => {
                return sp.GetRequiredService<IUrlHelperFactory>().GetUrlHelper(sp.GetRequiredService<IActionContextAccessor>().ActionContext);
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var authConfig = Configuration.GetSection("IdentityServer");
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options => {
                    // base-address of your identityserver
                    options.Authority = authConfig.GetValue<string>("Authority");
                    options.RequireHttpsMetadata = authConfig.GetValue<bool>("RequireHttpsMetadata");
                    options.RoleClaimType = "role";
                    options.NameClaimType = "name";

                    // name of the API resource
                    options.ApiName = authConfig.GetValue<string>("ApiName");
                    options.ApiSecret = authConfig.GetValue<string>("ApiSecret");

                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Version = "v1", Title = "Acme.ShoppingCart API" });
                //c.DescribeAllEnumsAsStrings();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddSingleton<IUserClient, UserClient.UserClient>();
            var userClientConfiguration = Configuration.GetSection("UserApi").Get<UserClientConfiguration>();
            services.AddSingleton(userClientConfiguration);
            services.AddPolicyServerRuntimeClient(Configuration.GetSection("PolicyServer"))
                .AddAuthorizationPermissionPolicies();
            services.AddTransient<ISubjectService, SubjectService>();

            // TODO: move to IServiceCollectionExtensions method
            services.AddOptions();
            services.AddHttpContextAccessor();
            services.AddTransient<ISubjectPrincipal, SubjectPrincipal>((sp) => {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

                // when there is no httpcontext available, assume that subject is the "service" itself
                var claims = new List<Claim>() {
                    new Claim(JwtRegisteredClaimNames.Sub, Guid.Empty.ToString())
                };
                var system = new SubjectPrincipal(claims);

                if (httpContextAccessor?.HttpContext != null) {
                    var principal = new SubjectPrincipal(httpContextAccessor.HttpContext.User);
                    if (principal.SubjectId != null) {
                        return principal;
                    }
                }

                return system;
            });

            services.AddSingleton(Configuration);
            bootstrapper.InitIoCContainer(Configuration as IConfigurationRoot, services);
            services.AddSingleton<IEncryptionService, EncryptionService>();

            //services.AddSingleton<IDomainEventHandler<ContractorStateChangedEvent>, ContractorStateChangedEventHandler>();

            // outbox hosted service
            var outboxConfiguration = Configuration.GetSection("OutboxHostedService").Get<OutboxHostedServiceConfiguration>();
            services.AddSingleton(outboxConfiguration);
            services.AddHostedService<OutboxHostedService<DatabaseContext>>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseMiniProfiler();
            app.UseMiddleware<CorrelationMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                const string s = "/swagger/v1/swagger.json";
                c.SwaggerEndpoint(s, "Acme.ShoppingCart Api V1");
            });

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
