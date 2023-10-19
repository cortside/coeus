using Acme.IdentityServer.WebApi.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.IdentityServer.WebApi.Controllers.Scope {

    /// <summary>
    /// Scopes v1 endpoints controller
    /// </summary>
    [Route("api/scopes")]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ScopeController : ControllerBase {
        private readonly IScopeService _scopeService;

        public ScopeController(IScopeService scopeService) {
            _scopeService = scopeService;
        }

        /// <summary>
        /// returns a list of strings with all the active ApiScopes
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public IActionResult GetAll() {
            var scopes = _scopeService.GetAll();
            return Ok(scopes);
        }
    }
}
