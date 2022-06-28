using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Cortside.IdentityServer.Services;
using Cortside.IdentityServer.WebApi.Controllers;
using Cortside.IdentityServer.WebApi.Services;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cortside.IdentityServer.Controllers.Account {
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    public class AccountController : Controller {
        private readonly ILogger<AccountController> logger;

        private readonly IAuthenticator authenticator;
        private readonly IConfigurationRoot config;
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly IAccountService accountService;
        private readonly IEventService eventService;
        private readonly IIdentityServerInteractionService idsService;
        private readonly IPersistedGrantService persistedGrantService;
        private readonly IUserService userService;

        public AccountController(
            ILogger<AccountController> logger,
            IAuthenticator authenticator,
            IConfigurationRoot config,
            IHttpContextAccessor httpContextAccessor,
            IAccountService accountService,
            IEventService eventService,
            IIdentityServerInteractionService idsService,
            IPersistedGrantService persistedGrantService,
            IUserService userService
        ) {
            this.logger = logger;

            this.config = config;
            this.httpContextAccessor = httpContextAccessor;
            this.authenticator = authenticator;

            this.accountService = accountService;
            this.eventService = eventService;
            this.idsService = idsService;
            this.userService = userService;
            this.persistedGrantService = persistedGrantService;
        }

        /// <summary>
        /// Show login page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl) {
            // build a model so we know what to show on the login page
            LoginViewModel vm = null;
            try {
                var forgotPasswordAddress = string.Format(config["forgotPasswordAddress"], returnUrl);
                vm = await accountService.BuildLoginViewModelAsync(returnUrl, forgotPasswordAddress);

                if (vm.IsExternalLoginOnly) {
                    // we only have one option for logging in and it's an external provider
                    return await ExternalLogin(vm.ExternalLoginScheme, returnUrl);
                }

            } catch (Exception ex) {
                logger.LogError($"Login exception {ex}");
            }
            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button) {
            if (button != "login") {
                // the user clicked the "cancel" button
                var context = await idsService.GetAuthorizationContextAsync(model.ReturnUrl);
                if (context != null) {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await idsService.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(model.ReturnUrl);
                } else {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password)) {
                ModelState.AddModelError("loginerror", AccountOptions.EmptyFieldErrorMessage);
            }

            if (ModelState.IsValid) {
                var loginInfo = new LoginInfo {
                    Username = model.Username,
                    Password = model.Password
                };
                var authResponse = await authenticator.AuthenticateAsync(loginInfo);
                var user = authResponse.User;

                if (user != null) {
                    AuthenticationProperties props = null;
                    if (AccountOptions.AllowRememberLogin && model.RememberLogin) {
                        props = new AuthenticationProperties {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                        };
                    }
                    var isuser = new IdentityServerUser(user.UserId.ToString()) {
                        DisplayName = user.Username,
                    };
                    await httpContextAccessor.HttpContext.SignInAsync(isuser, props);

                    var returnUrl = model.ReturnUrl;
                    logger.LogInformation($"Return URL: {returnUrl}");
                    if (idsService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl)) {
                        return Redirect(returnUrl);
                    }

                    return Redirect("~/");
                }

                await eventService.RaiseAsync(new UserLoginFailureEvent(model.Username, authResponse.Error));

                ModelState.AddModelError("loginerror", authResponse.Error);
            }

            // something went wrong, show form with error
            var vm = await accountService.BuildLoginViewModelAsync(model);
            return View(vm);
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl) {
            var props = new AuthenticationProperties() {
                RedirectUri = Url.Action("ExternalLoginCallback"),
                Items =
                {
                    { "returnUrl", returnUrl }
                }
            };

            // windows authentication needs special handling
            // since they don't support the redirect uri, 
            // so this URL is re-triggered when we call challenge
            if (AccountOptions.WindowsAuthenticationSchemeName == provider) {
                // see if windows auth has already been requested and succeeded
                var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
                if (result?.Principal is WindowsPrincipal wp) {
                    props.Items.Add("scheme", AccountOptions.WindowsAuthenticationSchemeName);

                    var id = new ClaimsIdentity(provider);
                    id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

                    // add the groups as claims -- be careful if the number of groups is too large
                    if (AccountOptions.IncludeWindowsGroups) {
                        var wi = wp.Identity as WindowsIdentity;
                        var groups = wi.Groups.Translate(typeof(NTAccount));
                        var roles = groups.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
                        id.AddClaims(roles);
                    }

                    await HttpContext.SignInAsync(
                        IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme,
                        new ClaimsPrincipal(id),
                        props);
                    return Redirect(props.RedirectUri);
                } else {
                    // challenge/trigger windows auth
                    return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
                }
            } else {
                // start challenge and roundtrip the return URL
                props.Items.Add("scheme", provider);
                return Challenge(props, provider);
            }
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult Challenge(string scheme, string returnUrl) {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (!Url.IsLocalUrl(returnUrl) && !idsService.IsValidReturnUrl(returnUrl)) {
                // user might have clicked on a malicious link - should be logged
                throw new InvalidOperationException("invalid return URL");
            }

            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties {
                RedirectUri = Url.Action(nameof(ExternalLoginCallback)),
                Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", scheme },
                }
            };

            return Challenge(props, scheme);
        }


        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback() {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true) {
                throw new InvalidOperationException("External authentication error");
            }

            // retrieve claims of the external user
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new InvalidOperationException("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var userId = userIdClaim.Value;

            // this is where custom logic would most likely be needed to match your users from the
            // external provider's authentication result, and provision the user as you see fit.
            // 
            // check if the external user is already provisioned
            var user = userService.FindByExternalProvider(provider, userId);
            if (user == null) {
                // this sample simply auto-provisions new external user
                // another common approach is to start a registrations workflow first
                logger.LogDebug($"Creating new external user: {userId}");
                user = await userService.AutoProvisionUser(provider, userId, claims);
                
            } else {
                // update the existing user claims that we "cache"
                logger.LogDebug($"Updating existing user: {userId}");
                user = await userService.UpdateExternalUserClaims(provider, userId, claims);
            }

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            var isuser = new IdentityServerUser(user.UserId.ToString()) {
                DisplayName = user.Username,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(isuser, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // check if external login is in the context of an OIDC request
            var context = await idsService.GetAuthorizationContextAsync(returnUrl);
            await eventService.RaiseAsync(new UserLoginSuccessEvent(provider, user.ProviderSubjectId, user.UserId.ToString(), user.Username, true, context?.Client.ClientId));

            // validate return URL and redirect back to authorization endpoint or a local page
            if (idsService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
        // this will be different for WS-Fed, SAML2p or other protocols
        private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps) {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null) {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null) {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId) {
            // build a model so the logout page knows what to display
            var vm = await accountService.BuildLogoutViewModelAsync(logoutId);

            if (!vm.ShowLogoutPrompt) {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model) {
            // build a model so the logged out page knows what to display
            var vm = await accountService.BuildLoggedOutViewModelAsync(model.LogoutId);

            var user = httpContextAccessor.HttpContext.User;
            if (user?.Identity.IsAuthenticated == true) {
                // delete local authentication cookie
                await httpContextAccessor.HttpContext.SignOutAsync();

                // remove persisted grants from store
                var logoutContext = await idsService.GetLogoutContextAsync(vm.LogoutId);
                if (logoutContext.ClientIds != null) {
                    foreach (var clientId in logoutContext.ClientIds) {
                        await persistedGrantService.RemoveAllGrantsAsync(user.GetSubjectId(), clientId);
                    }
                }

                // raise the logout event
                await eventService.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout) {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }
    }
}
