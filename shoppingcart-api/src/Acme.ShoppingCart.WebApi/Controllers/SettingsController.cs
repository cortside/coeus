#pragma warning disable S1135

// TODO: consider moving this to cortside.aspnetcore
// TODO: consider using ISettingsModel that is registered in DI and letting that serialize the actual object

using System;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Asp.Versioning;
using Cortside.Health.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Acme.ShoppingCart.WebApi.Controllers {
    /// <summary>
    /// Controller to handle settings related requests
    /// </summary>
    [ApiVersionNeutral]
    [Route("api/settings")]
    [ApiController]
    [Produces("application/json")]
    public class SettingsController : ControllerBase {
        /// <summary>
        /// Gets the application configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsController"/> class
        /// </summary>
        /// <param name="configuration">The application configuration</param>
        public SettingsController(IConfiguration configuration) {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the service settings that a consumer may need to be aware of
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the settings model</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(SettingsModel), 200)]
        [ResponseCache(CacheProfileName = "Default")]
        public IActionResult Get() {
            var result = GetSettingsModel();
            return Ok(result);
        }

        /// <summary>
        /// Creates and returns a <see cref="SettingsModel"/> with the current configuration values
        /// </summary>
        /// <returns>A <see cref="SettingsModel"/> containing the current configuration values</returns>
        private SettingsModel GetSettingsModel() {
            var serviceBus = Configuration.GetSection("ServiceBus");
            var authConfig = Configuration.GetSection("IdentityServer");
            var policyServer = Configuration.GetSection("PolicyServer");
            var build = Configuration.GetSection("Build");

            return new SettingsModel() {
                Build = new BuildModel() {
                    Version = build.GetValue<string>("version"),
                    Timestamp = build.GetValue<DateTime>("timestamp"),
                    Tag = build.GetValue<string>("tag"),
                    Suffix = build.GetValue<string>("suffix")
                },
                Configuration = new ConfigurationModel() {
                    ServiceBus = new ServiceBusModel {
                        Exchange = serviceBus.GetValue<string>("Exchange"),
                        NameSpace = serviceBus.GetValue<string>("Namespace"),
                        Queue = serviceBus.GetValue<string>("Queue"),
                    },
                    IdentityServer = new IdentityServerModel {
                        Apiname = authConfig.GetValue<string>("ApiName"),
                        Authority = authConfig.GetValue<string>("Authority"),
                        BaseUrl = authConfig.GetValue<string>("BaseUrl")
                    },
                    PolicyServer = new PolicyServerModel {
                        BasePolicy = policyServer.GetValue<string>("BasePolicy"),
                        Url = policyServer.GetValue<string>("PolicyServerUrl"),
                    }
                },
                Service = Configuration["Service:Name"]
            };
        }
    }
}
