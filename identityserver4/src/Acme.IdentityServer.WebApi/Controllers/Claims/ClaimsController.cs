using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.IdentityServer.WebApi.Controllers.Claims {
    /// <summary>
    /// This sample controller allows a user to view list of their claims
    /// </summary>
    [SecurityHeaders]
    [Authorize(AuthenticationSchemes = IdentityServer4.IdentityServerConstants.DefaultCookieAuthenticationScheme)]
    public class ClaimsController : Controller {
        private readonly IProfileService profileService;

        public ClaimsController(IProfileService profileService) {
            this.profileService = profileService;
        }

        /// <summary>
        /// Show list of claims
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index() {
            var ctx = new ProfileDataRequestContext {
                Subject = User,
                RequestedClaimTypes = new string[] { },
                ValidatedRequest = new IdentityServer4.Validation.ValidatedRequest() { ClientClaims = new List<Claim>() }
            };
            await profileService.GetProfileDataAsync(ctx);

            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(ctx.IssuedClaims));

            return View("Index");
        }
    }
}
