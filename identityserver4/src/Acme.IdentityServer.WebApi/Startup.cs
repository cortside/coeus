using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Cortside.Common.Messages.Filters;
using Cortside.DomainEvent;
using Cortside.DomainEvent.EntityFramework;
using Cortside.DomainEvent.EntityFramework.Hosting;
using Cortside.Health;
using Cortside.Health.Checks;
using Cortside.Health.Controllers;
using Cortside.Health.Models;
using Cortside.Health.Recorders;
using Acme.IdentityServer.Controllers.Account;
using Acme.IdentityServer.Data;
using Acme.IdentityServer.EventHandlers;
using Acme.IdentityServer.Models;
using Acme.IdentityServer.Services;
using Acme.IdentityServer.WebApi.Assemblers;
using Acme.IdentityServer.WebApi.Assemblers.Implementors;
using Acme.IdentityServer.WebApi.Controllers;
using Acme.IdentityServer.WebApi.Controllers.ResetClientSecretController;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Helpers;
using Acme.IdentityServer.WebApi.Json;
using Acme.IdentityServer.WebApi.Security;
using Acme.IdentityServer.WebApi.Services;
using Acme.IdentityServer.WebApi.Services.ExtensionGrantValidators;
using IdentityServer4;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Acme.IdentityServer.WebApi {

    public class Startup {

        public Startup(IWebHostEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.local.json", optional: true)
                .AddJsonFile("build.json", false, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        //This was added so that TestServer in the Integration tests project would work
        public static HttpMessageHandler Handler { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // app insights -- must be first thing registered
            services.AddSingleton<ITelemetryInitializer, AppInsightsInitializer>();
            services.AddApplicationInsightsTelemetry(o => {
                o.InstrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"];
                o.EnableAdaptiveSampling = bool.Parse(Configuration["ApplicationInsights:EnableAdaptiveSampling"]);
                o.EnableActiveTelemetryConfigurationSetup = true;
            });

            services.AddRazorPages();

            // Add cors policies
            services.AddCors();

            // Add framework services.
            services.AddControllers(options => {
                options.Filters.Add<MessageExceptionResponseFilter>();
            })
            .AddNewtonsoftJson(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.Converters.Add(new IsoTimeSpanConverter());
            }).PartManager.ApplicationParts.Add(new AssemblyPart(typeof(HealthController).Assembly));

            services.AddSingleton(Configuration);

            services.AddHttpClient<IGoogleRecaptchaV3Service, GoogleRecaptchaV3Service>();

            ConfigureLocalServices(services);
            ConfigureIdentityServer(services);
        }

        private void ConfigureLocalServices(IServiceCollection services) {
            var build = Configuration.GetSection("Build").Get<Build>();
            services.AddSingleton(build == null ? new Build() : build);

            ConfigureServiceBus(services);

            services.AddSingleton<IHashProvider, HashProvider>();
            services.AddSingleton<IAuthenticator, Authenticator>();
            services.AddSingleton<IHtmlTemplateLocationFinder, HtmlTemplateLocationFinder>();

            services.AddScoped<IHtmlTemplateHelper, HtmlTemplateHelper>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IScopeService, ScopeService>();
            services.AddScoped<IClientSecretService, ClientSecretService>();
            services.AddScoped<IResetClientSecretService, ResetClientSecretService>();
            services.AddScoped<IUserModelAssembler, UserModelAssembler>();
            services.AddScoped<IFileSystem, FileSystem>();
            services.AddScoped<IPhoneNumberHelper, PhoneNumberHelper>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddTransient<ISubjectPrincipal, SubjectPrincipal>((sp) => {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                if (httpContextAccessor.HttpContext != null) {
                    return new SubjectPrincipal(httpContextAccessor.HttpContext.User);
                } else {
                    return new SubjectPrincipal();
                }
            });

            services.AddScoped<IUserService, UserService>();

            Assembly.GetEntryAssembly().GetTypes()
                .Where(x => (x.Name.EndsWith("Handler") || x.Name.EndsWith("Factory"))
                    && x.GetTypeInfo().IsClass
                    && !x.GetTypeInfo().IsAbstract
                    && x.GetInterfaces().Any())
                .ToList().ForEach(x => {
                    x.GetInterfaces().ToList()
                        .ForEach(i => services.AddSingleton(i, x));
                });

            // health checks
            // telemetry recorder
            string instrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"];
            string endpointAddress = Configuration["ApplicationInsights:EndpointAddress"];
            if (!string.IsNullOrEmpty(instrumentationKey) && !string.IsNullOrEmpty(endpointAddress)) {
                TelemetryConfiguration telemetryConfiguration = new TelemetryConfiguration(instrumentationKey, new InMemoryChannel { EndpointAddress = endpointAddress });
                TelemetryClient telemetryClient = new TelemetryClient(telemetryConfiguration);
                services.AddSingleton(telemetryClient);
                services.AddTransient<IAvailabilityRecorder, ApplicationInsightsRecorder>();
            } else {
                services.AddTransient<IAvailabilityRecorder, NullRecorder>();
            }

            var hchsconfig = Configuration.GetSection("HealthCheckHostedService").Get<HealthCheckServiceConfiguration>();
            services.AddSingleton(hchsconfig);
            services.AddSingleton(Configuration.GetSection("Build").Get<BuildModel>());
            services.AddTransient<ICheckFactory, CheckFactory>();
            services.AddHostedService<HealthCheckHostedService>();

            // checks
            services.AddTransient<UrlCheck>();
            services.AddTransient<DbContextCheck>();

            // check factory and hosted service
            services.AddSingleton(sp => {
                var cache = sp.GetService<IMemoryCache>();
                var logger = sp.GetService<ILogger<Check>>();
                var recorder = sp.GetService<IAvailabilityRecorder>();
                var configuration = sp.GetService<IConfiguration>();

                var factory = new CheckFactory(cache, logger, recorder, sp, configuration);
                return factory as ICheckFactory;
            });

            // for DbContextCheck
            services.AddTransient<DbContext, IdentityServerDbContext>();
        }

        private void ConfigureServiceBus(IServiceCollection services) {
            var config = Configuration.GetSection("ServiceBus");
            var pSettings = new DomainEventPublisherSettings() {
                Topic = config.GetValue<string>("Exchange"),
                AppName = config.GetValue<string>("AppName"),
                Protocol = config.GetValue<string>("Protocol"),
                PolicyName = config.GetValue<string>("Policy"),
                Key = config.GetValue<string>("Key"),
                Namespace = config.GetValue<string>("Namespace"),
                Durable = 1,
                Credits = config.GetValue<int>("Credits")
            };

            var rSettings = new DomainEventReceiverSettings() {
                Queue = config.GetValue<string>("Queue"),
                AppName = config.GetValue<string>("AppName"),
                Protocol = config.GetValue<string>("Protocol"),
                PolicyName = config.GetValue<string>("Policy"),
                Key = config.GetValue<string>("Key"),
                Namespace = config.GetValue<string>("Namespace"),
                Durable = 1,
                Credits = config.GetValue<int>("Credits"),
            };

            services.AddSingleton(pSettings);
            services.AddSingleton(rSettings);

            // outbox hosted service
            services.AddSingleton(Configuration.GetSection("OutboxHostedService").Get<OutboxHostedServiceConfiguration>());
            services.AddHostedService<OutboxHostedService<IdentityServerDbContext>>();

            // message publisher/receivers
            services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
            services.AddSingleton<IDomainEventReceiver, DomainEventReceiver>();
            services.AddTransient<IDomainEventOutboxPublisher, DomainEventOutboxPublisher<IdentityServerDbContext>>();
        }

        private IDictionary<string, Type> RegisterMessageTypes() {
            return new Dictionary<string, Type> {
                { "Chiron.Registration.Customer.Event.CustomerRegisteredEvent", typeof(UserRegisteredEvent) },
                { "Chiron.Registration.Clerk.Event.ClerkRegisteredEvent", typeof(UserRegisteredEvent) }
            };
        }

        private void ConfigureIdentityServer(IServiceCollection services) {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var authConnString = Configuration.GetConnectionString("AuthConnString");
            services.AddDbContext<IdentityServerDbContext>(o => {
                o.UseSqlServer(authConnString);
            }, ServiceLifetime.Scoped);
            services.AddScoped<IIdentityServerDbContext>(x => x.GetRequiredService<IdentityServerDbContext>());

            // Add a DbContext to store your Database Keys
            services.AddDbContext<DataProtectionKeyContext>(options =>
                options.UseSqlServer(authConnString)
            );

            services.AddDataProtection().PersistKeysToDbContext<DataProtectionKeyContext>();

            IIdentityServerBuilder idsBuilder = services.AddIdentityServer(options => {
                if (!string.IsNullOrWhiteSpace(Configuration["issuerUri"])) {
                    options.IssuerUri = Configuration["issuerUri"];
                }

                options.AccessTokenJwtType = "JWT";
                // this has to be set otherwise aud is not set
                options.EmitStaticAudienceClaim = true;
            });

            idsBuilder.AddSigningCredential(new X509Certificate2(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "IdentityServer.pfx")))
                .AddConfigurationStore(options => {
                    options.DefaultSchema = "auth";
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(authConnString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options => {
                    options.DefaultSchema = "auth";
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(authConnString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                })
                .AddProfileService<UserProfileService>()
                .AddCustomTokenRequestValidator<DelegationTokenRequestValidator>()
                .AddExtensionGrantValidator<RecaptchaGrantValidator>()
                .AddExtensionGrantValidator<DelegationGrantValidator>();

            var authenticationBuilder = services.AddAuthentication();

            var providers = Configuration.GetSection("ExternalProviders").Get<List<Provider>>();
            services.AddSingleton(providers);

            foreach (var provider in providers) {
                authenticationBuilder.AddOpenIdConnect(provider.Scheme, provider.DisplayName, options => {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.Authority = provider.Authority;
                    options.ClientId = provider.ClientId;
                    options.CallbackPath = provider.CallbackPath;
                    foreach (var scope in provider.Scopes) {
                        options.Scope.Add(scope);
                    }
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = false
                    };
                    options.GetClaimsFromUserInfoEndpoint = true;
                    if (provider.AddIdTokenHint) {
                        options.SaveTokens = true;
                        options.Events.OnRedirectToIdentityProviderForSignOut += (Func<RedirectContext, Task>)(async context => {
                            context.ProtocolMessage.IdTokenHint = await context.HttpContext.GetTokenAsync(OpenIdConnectResponseType.IdToken);
                        });
                    }
                });
            }

            authenticationBuilder.AddIdentityServerAuthentication(options => {
                // configure authentication for non-openid endpoints
                options.SupportedTokens = SupportedTokens.Both;
                options.RequireHttpsMetadata = bool.Parse(Configuration["authentication:requireHttpsMetadata"]);
                options.Authority = Configuration["authentication:authority"];
                options.ApiName = Configuration["authentication:apiResource"];
                // for reference tokens to work correctly, secret needs to match with a SharedSecret in db
                // actual secret value in the db needs to be a matching base64 string of the SHA256/SHA512 hash, see HashedSharedSecretValidator.ValidateAsync() in ids source code
                options.ApiSecret = Configuration["authentication:apiResourceSecret"];
                //These 3 handlers were added so that TestServer in the Integration tests project would work
                options.JwtBackChannelHandler = Handler;

                options.ClaimsIssuer = Configuration["authentication:authority"];
            });

            services.AddMvc();

            services.AddSwaggerGen(swagger => {
                swagger.DescribeAllParametersInCamelCase();
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityServerSwagger" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {
            // added for okta -- not sure if it's needed
            app.UseCookiePolicy(new CookiePolicyOptions {
                Secure = CookieSecurePolicy.Always
            });

            app.UseSerilogRequestLogging();
            var logger = loggerFactory.CreateLogger<Startup>();
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
                    logger.LogInformation($"X-Forwarded-Prefix={prefix}, Path={context.Request.Path}");
                    context.Request.PathBase = PathString.FromUriComponent(prefix.ToString());
                    // TODO: subtract PathBase from Path if needed.
                }

                // IDS origin has to be set in the middleware now
                if (!string.IsNullOrWhiteSpace(Configuration["publicOrigin"])) {
                    context.SetIdentityServerOrigin(Configuration["publicOrigin"]);
                }
                return next();
            });

            // to handle x-forwarded-* headers
            var fordwardedHeaderOptions = new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto
            };
            fordwardedHeaderOptions.KnownNetworks.Clear();
            fordwardedHeaderOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(fordwardedHeaderOptions);
            app.UseStaticFiles();
            app.UseIdentityServer();

            RegisterMessageTypes();
            app.UseRouting();
            app.UseCors(builder => builder
                .WithOrigins(Configuration.GetSection("Cors").GetSection("Origins").Get<string[]>())
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(
                endpoints => {
                    endpoints.MapControllers();
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapRazorPages();
                });
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityServerSwagger");
            });
        }
    }
}
