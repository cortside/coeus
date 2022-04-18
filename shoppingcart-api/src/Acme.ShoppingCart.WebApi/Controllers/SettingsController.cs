using System;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Cortside.Health.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Acme.ShoppingCart.WebApi.Controllers {
    /// <summary>
    /// Settings
    /// </summary>
    [ApiVersionNeutral]
    [Route("api/settings")]
    [ApiController]
    [Produces("application/json")]
    public class SettingsController : ControllerBase {
        /// <summary>
        /// Config
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// SettingsController constructor
        /// </summary>
        public SettingsController(IConfiguration configuration) {
            Configuration = configuration;
        }

        /// <summary>
        /// Service settings that a consumer may need to be aware of
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(SettingsModel), 200)]
        [ResponseCache(CacheProfileName = "Default")]
        public IActionResult Get() {
            var result = GetSettingsModel();
            return Ok(result);
        }

        private SettingsModel GetSettingsModel() {
            var ServiceBus = Configuration.GetSection("ServiceBus");
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
                    ServiceBus = new ServicebusModel {
                        Exchange = ServiceBus.GetValue<string>("Exchange"),
                        NameSpace = ServiceBus.GetValue<string>("Namespace"),
                        Queue = ServiceBus.GetValue<string>("Queue"),
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
                Service = "ShoppingCart"
            };
        }
    }
}
