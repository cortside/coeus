using System;
using System.Net.Http;
using System.Threading.Tasks;
using Acme.IdentityServer.BootStrap;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Installers;
using Cortside.AspNetCore;
using Cortside.AspNetCore.ApplicationInsights;
using Cortside.AspNetCore.Auditable;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.AspNetCore.Builder;
using Cortside.AspNetCore.EntityFramework;
using Cortside.AspNetCore.Middleware;
using Cortside.AspNetCore.Swagger;
using Cortside.Common.Correlation;
using Cortside.DomainEvent;
using Cortside.DomainEvent.EntityFramework;
using Cortside.Health;
using IdentityServer4.Extensions;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using Serilog;

namespace Acme.IdentityServer.WebApi {
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

        //This was added so that TestServer in the Integration tests project would work
        public static HttpMessageHandler Handler { get; set; }

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
            services.AddDatabaseContext<IIdentityServerDbContext, IdentityServerDbContext>(Configuration);

            // add health services
            services.AddHealth(o => {
                o.UseConfiguration(Configuration);
            });

            // add domain event receiver with handlers
            services.AddDomainEventReceiver(o => {
                o.UseConfiguration(Configuration);
            });

            // add domain event publish with outbox
            services.AddDomainEventOutboxPublisher<IdentityServerDbContext>(Configuration);

            // add controllers and all of the api defaults
            services.AddApiDefaults();

            // add SubjectPrincipal for auditing
            services.AddSubjectPrincipal();
            services.AddTransient<ISubjectFactory<Subject>, DefaultSubjectFactory>();

            // Add swagger with versioning and OpenID Connect configuration using Newtonsoft
            services.AddSwagger(Configuration, "IdentityServer API", "IdentityServer API", new[] { "v1" });

            // from Startup before update
            services.AddCors();
            services.AddRazorPages();
            services.AddMvc();

            // setup and register boostrapper and it's installers -- needs to be last
            services.AddBootStrapper<DefaultApplicationBootStrapper>(Configuration, o => {
                o.AddInstaller(new DomainServiceInstaller());
                o.AddInstaller(new IdentityServerInstaller());
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider) {
            var logger = Log.Logger;

            //app.UseApiDefaults(Configuration); -- without cors
            app.UseMiddleware<CorrelationMiddleware>();
            app.UseMiddleware<RequestIpAddressLoggingMiddleware>();
            app.UseExceptionHandler((Action<IApplicationBuilder>)(error => error.Run((RequestDelegate)(_ => Task.CompletedTask))));
            app.UseResponseCompression();
            app.UseResponseCaching();
            app.UseSerilogRequestLogging();

            // handle cookies in chromium browsers over http
            if (bool.Parse(Configuration["UseInsecureCookies"])) {
                app.UseCookiePolicy(new CookiePolicyOptions {
                    OnAppendCookie = cookieContext => {
                        cookieContext.CookieOptions.SameSite = SameSiteMode.Unspecified;
                        cookieContext.CookieOptions.Secure = false;
                    },
                    OnDeleteCookie = cookieContext => {
                        cookieContext.CookieOptions.SameSite = SameSiteMode.Unspecified;
                        cookieContext.CookieOptions.Secure = false;
                    }
                });
            } else {
                app.UseCookiePolicy();
            }

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                IdentityModelEventSource.ShowPII = true;
            } else {
                app.UseExceptionHandler("/Home/Error");
            }

            // to handle x-forwarded-prefix header when backend is mapped to virtual path on frontend
            app.Use((context, next) => {
                var prefix = context.Request.Headers["x-forwarded-prefix"];
                if (!StringValues.IsNullOrEmpty(prefix)) {
                    logger.Information($"X-Forwarded-Prefix={prefix}, Path={context.Request.Path}");
                    context.Request.PathBase = PathString.FromUriComponent(prefix.ToString());
                    // TODO: subtract PathBase from Path if needed.
                }

                // IDS origin has to be set in the middleware now
                if (!string.IsNullOrWhiteSpace(Configuration["publicOrigin"])) {
                    context.SetIdentityServerOrigin(Configuration["publicOrigin"]);
                }
                return next();
            });

            // TODO: add extension method for UseReverseProxyHeaders
            // in the method docs state what headers need to be set
            // to handle x-forwarded-* headers
            var forwardedHeaderOptions = new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };
            forwardedHeaderOptions.KnownNetworks.Clear();
            forwardedHeaderOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeaderOptions);

            app.UseStaticFiles();

            // must be after forward headers
            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization(); // intentionally set after UseRouting
            app.UseSubjectPrincipal(); // intentionally set after UseAuthentication

            //app.UseApiDefaults(Configuration);
            app.UseCors((Action<CorsPolicyBuilder>)(builder => builder.WithOrigins(Configuration.GetSection("Cors").GetSection("Origins").Get<string[]>()).SetIsOriginAllowedToAllowWildcardSubdomains().AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

            app.UseEndpoints(
                endpoints => {
                    endpoints.MapControllers();
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapRazorPages();
                });
            app.UseSwagger("IdentityServer API", provider);
        }
    }
}
