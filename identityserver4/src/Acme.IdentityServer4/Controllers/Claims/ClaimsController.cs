using Cortside.IdentityServer.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cortside.IdentityServer.WebApi.Controllers.Claims {
    /// <summary>
    /// This sample controller allows a user to view list of their claims
    /// </summary>
    [SecurityHeaders]
    [Authorize(AuthenticationSchemes = IdentityServer4.IdentityServerConstants.DefaultCookieAuthenticationScheme)]
    public class ClaimsController : Controller {

        public ClaimsController() {
        }

        /// <summary>
        /// Show list of claims
        /// </summary>
        [HttpGet]
        public IActionResult Index() {
            return View("Index");
        }

    }
}
