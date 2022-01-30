using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Cortside.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PolicyServer.Runtime.Client;

namespace Acme.ShoppingCart.WebApi.Controllers {
    /// <summary>
    /// provides resources from the policy server
    /// </summary>
    [Route("api/v1/authorization")]
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Authorize]
    public class AuthorizationController : ControllerBase {
        private readonly ILogger<AuthorizationController> logger;
        private readonly IPolicyServerRuntimeClient policyClient;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Authorization controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="client"></param>
        /// <param name="configuration"></param>
        public AuthorizationController(ILogger<AuthorizationController> logger, IPolicyServerRuntimeClient client, IConfiguration configuration) {
            this.logger = logger;
            this.policyClient = client;
            this.configuration = configuration;
        }

        /// <summary>
        /// Gets the list if permissions associated with the caller, determined by their bearer token
        /// </summary>
        /// <returns>The list of permissions</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(AuthorizationModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPermissionsAsync() {
            logger.LogInformation("Retrieving authorization permissions for user.");
            var authProperties = await policyClient.EvaluateAsync(User).ConfigureAwait(false);
            AuthorizationModel responseModel = new AuthorizationModel() {
                Roles = authProperties.Roles.ToList(),
                Permissions = authProperties.Permissions.ToList()
            };
            var permissionsPrefix = configuration.GetSection("PolicyServer").GetValue<string>("BasePolicyPrefix");
            responseModel.Roles = responseModel.Roles.Select(p => $"{permissionsPrefix}.{p}").ToList();
            responseModel.Permissions = responseModel.Permissions.Select(p => $"{permissionsPrefix}.{p}").ToList();
            responseModel.Principal = SubjectPrincipal.From(ControllerContext.HttpContext.User);
            return Ok(responseModel);
        }
    }
}
