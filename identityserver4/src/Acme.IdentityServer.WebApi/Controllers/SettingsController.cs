using Cortside.DomainEvent;
using Acme.IdentityServer.WebApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Acme.IdentityServer.WebApi.Controllers {
    /// <summary>
    /// Controller for settings
    /// </summary>
    [EnableCors("AllowAll")]
    [Route("api/users")]
    [ApiVersionNeutral]
    [ApiController]
    [Produces("application/json")]
    public class SettingsController : Controller {
        private readonly DomainEventPublisherSettings options;
        private readonly Build build;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="build"></param>
        public SettingsController(DomainEventPublisherSettings options, Build build) {
            this.options = options;
            this.build = build;
        }

        /// <summary>
        /// Return configuration settings for service (which may or may not be safe)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Settings Get() {
            var s = new Settings();

            s.Deployment = build?.Tag;
            s.App = "IdentityServer";
            s.Build = build;
            s.Config = new ConfigSettings() {
                Namespace = options.Namespace,
                Policy = options.PolicyName
            };

            return s;
        }
    }
}
