using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.AuditEvents;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Events;
using Acme.IdentityServer.WebApi.Services;
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
using Newtonsoft.Json;
using Serilog;

namespace Acme.IdentityServer.WebApi.Controllers.Account {
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    public class AccountController : Controller {
        private readonly ILogger<AccountController> logger;

        private readonly IAuthenticator authenticator;
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly IAccountService accountService;
        private readonly IEventService eventService;
        private readonly IIdentityServerInteractionService idsService;
        private readonly IPersistedGrantService persistedGrantService;
        private readonly IUserService userService;
        private readonly List<Provider> providers;

        public AccountController(
            ILogger<AccountController> logger,
            IAuthenticator authenticator,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            IAccountService accountService,
            IEventService eventService,
            IIdentityServerInteractionService idsService,
            IPersistedGrantService persistedGrantService,
            IUserService userService,
            List<Provider> providers) {
            this.logger = logger;

            this.config = config;
            this.httpContextAccessor = httpContextAccessor;
            this.authenticator = authenticator;

            this.accountService = accountService;
            this.eventService = eventService;
            this.idsService = idsService;
            this.userService = userService;
            this.persistedGrantService = persistedGrantService;
            this.providers = providers;
        }

        /// <summary>
        /// Show login page, called before actual login
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
            if (button != "login" && button != "check2fa") {
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

            if (button == "check2fa" && ModelState.IsValid) {
                var loginInfo = new LoginInfo {
                    Username = model.Username,
                    Password = model.Password
                };
                var authResponse = await authenticator.AuthenticateAsync(loginInfo);
                var user = authResponse.User;

                if (user != null && !string.IsNullOrEmpty(model.TwoFactorCode)) {
                    if (userService.VerifyCurrentTOTP(user.UserId, model.TwoFactorCode)) {
                        user.TwoFactor = model.SetupCode;
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

                        IList<string> amrs = new List<string>();
                        amrs.Add("pwd");
                        amrs.Add("otp");
                        string amrJson = JsonConvert.SerializeObject(amrs);

                        isuser.AdditionalClaims.Add(new Claim("amr", amrJson, IdentityServerConstants.ClaimValueTypes.Json));
                        await httpContextAccessor.HttpContext.SignInAsync(isuser, props);

                        // TODO: log siem event for success
                        Log.ForContext<UserLoginAuditEvent>().Information("User {Username} logged in as local user with {Result}", user.Username, "success");

                        var returnUrl = model.ReturnUrl;
                        logger.LogInformation($"Return URL: {returnUrl}");
                        if (idsService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl)) {
                            return Redirect(returnUrl);
                        }
                        return Redirect("~/");
                    }

                    var vm2fa = await accountService.BuildLoginViewModelAsync(model);
                    vm2fa.ValidatedLogin = true;
                    vm2fa.HasTwoFactorSetup = true;
                    vm2fa.Password = model.Password;
                    await eventService.RaiseAsync(new UserLoginFailureEvent(model.Username, "Invalid Code, please try again"));

                    // TODO: log siem event for failure
                    Log.ForContext<UserLoginAuditEvent>().Information("User {Username} logged in with {Result} for {Reason}", model.Username, "failure", "Invalid Code, please try again");

                    ModelState.AddModelError("loginerror", "Invalid Code, please try again");
                    return View(vm2fa);
                }

                await eventService.RaiseAsync(new UserLoginFailureEvent(model.Username, authResponse.Error));

                // TODO: log siem event for failure
                Log.ForContext<UserLoginAuditEvent>().Information("User {Username} logged in with {Result} for {Reason}", model.Username, "failure", authResponse.Error);

                ModelState.AddModelError("loginerror", authResponse.Error);
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
                    if (user.TwoFactorVerified) {
                        //Prompt for 2FA code check
                        var vm2fa = await accountService.BuildLoginViewModelAsync(model);
                        vm2fa.ValidatedLogin = true;
                        vm2fa.HasTwoFactorSetup = true;
                        vm2fa.Password = model.Password;
                        return View(vm2fa);
                    } else {
                        if (!bool.Parse(config["requireTOTPForLocal"])) {
                            await SignInUserAsync(AccountOptions.AllowRememberLogin, model.RememberLogin, AccountOptions.RememberMeLoginDuration, user);

                            // TODO: log siem event for success
                            Log.ForContext<UserLoginAuditEvent>().Information("User {Username} logged in as local user with {Result}", user.Username, "success");

                            var returnUrl = model.ReturnUrl;
                            logger.LogInformation($"Return URL: {returnUrl}");
                            if (idsService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl)) {
                                return Redirect(returnUrl);
                            }

                            return Redirect("~/");
                        } else {
                            //redirect to 2FA Setup
                            var vm2faSetup = await accountService.BuildLoginViewModelAsync(model);
                            vm2faSetup.ValidatedLogin = true;
                            vm2faSetup.HasTwoFactorSetup = false;
                            //only generate this during setup.  Should not do this without user in process of setting up 2FA
                            //save code as an unverified code with SaveChanges
                            var twoFactorSetupCode = userService.GenerateAndSetNewTOTPCode(user.UserId);
                            var authenticatorUri = userService.GetTwoFactorURI(user.UserId);
                            vm2faSetup.Password = model.Password;
                            vm2faSetup.SetupCode = twoFactorSetupCode;
                            vm2faSetup.AuthenticatorUri = authenticatorUri;
                            return View(vm2faSetup);
                        }
                    }
                }

                await eventService.RaiseAsync(new UserLoginFailureEvent(model.Username, authResponse.Error));

                // TODO: log siem event for failure
                Log.ForContext<UserLoginAuditEvent>().Information("User {Username} logged in with {Result} for {Reason}", model.Username, "failure", authResponse.Error);

                ModelState.AddModelError("loginerror", authResponse.Error);
            }

            // something went wrong, show form with error
            var vm = await accountService.BuildLoginViewModelAsync(model);
            return View(vm);
        }

        private Task SignInUserAsync(bool allowRememberLogin, bool rememberLogin, TimeSpan rememberMeLoginDuration, User user) {
            AuthenticationProperties props = null;
            if (allowRememberLogin && rememberLogin) {
                props = new AuthenticationProperties {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(rememberMeLoginDuration)
                };
            }
            var isuser = new IdentityServerUser(user.UserId.ToString()) {
                DisplayName = user.Username,
            };
            return httpContextAccessor.HttpContext.SignInAsync(isuser, props);
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
                // TODO: log siem event for failure??
                throw new InvalidOperationException("External authentication error", result?.Failure);
            }

            // retrieve claims of the external user
            var externalUser = result.Principal;

            // get the scheme to find the right provider definition
            var scheme = result.Properties.Items["scheme"];
            var provider = providers.First(x => x.Scheme == scheme);

            // translate claim types
            var externalUserClaims = new List<Claim>();
            foreach (var claim in externalUser.Claims) {
                string translatedClaimType = claim.Type;
                if (claim.Type == ClaimTypes.Name) {
                    translatedClaimType = JwtClaimTypes.Name;
                } else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type)) {
                    translatedClaimType = JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type];
                }
                externalUserClaims.Add(new Claim(translatedClaimType, claim.Value));
            }

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userId = externalUserClaims.FirstOrDefault(x => x.Type == provider.ProviderIdClaim).Value ??
                              throw new InvalidOperationException("Unknown userid");
            // TODO: log siem event for failure ^?

            // cleanup name claim if there are multiple and one matches upn claim
            var upn = externalUserClaims.FirstOrDefault(x => x.Type == "upn");
            if (upn != null && externalUserClaims.Count(x => x.Type == JwtClaimTypes.Name) > 1) {
                var username = externalUserClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name && x.Value == upn.Value);
                if (username != null) {
                    externalUserClaims.Remove(username);
                }
            }

            // get the claims of interest from the external provider
            var providerClaims = new List<string>();
            providerClaims.AddRange(provider.Claims);
            providerClaims.Add("upn");
            var claims = externalUserClaims.Where(claim => providerClaims.Contains(claim.Type)).ToList();

            // this is where custom logic would most likely be needed to match your users from the
            // external provider's authentication result, and provision the user as you see fit.
            // 
            // check if the external user is already provisioned
            var user = userService.FindByExternalProvider(scheme, userId);
            if (user == null) {
                // this sample simply auto-provisions new external user
                // another common approach is to start a registrations workflow first
                logger.LogDebug($"Creating new external user: {userId}");
                user = await userService.AutoProvisionUser(scheme, userId, claims);
            } else {
                // update the existing user claims that we "cache"
                logger.LogDebug($"Updating existing user: {userId}");
                user = await userService.UpdateExternalUserClaims(scheme, userId, claims);
            }

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            // TODO: this could be where IdTokenHint could be defined -- and a variable in the provider definition
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            var isuser = new IdentityServerUser(user.UserId.ToString()) {
                DisplayName = user.UserClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value,
                IdentityProvider = scheme,
                AdditionalClaims = additionalLocalClaims
            };

            await HttpContext.SignInAsync(isuser, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // check if external login is in the context of an OIDC request
            var context = await idsService.GetAuthorizationContextAsync(returnUrl);
            await eventService.RaiseAsync(new UserLoginSuccessEvent(scheme, user.ProviderSubjectId, user.UserId.ToString(), user.Username, true, context?.Client.ClientId));

            // TODO: log siem event for success
            Log.ForContext<UserLoginAuditEvent>().Information("User {Username} logged in with external provider {ProviderName} with {Result} to client {ClientId}", user.Username, scheme, "success", context?.Client?.ClientId);

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
                var u = await userService.FindBySubjectIdAsync(Guid.Parse(user.GetSubjectId()));
                await eventService.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetDisplayName(), u?.Username));

                // TODO: log siem event for success
                Log.ForContext<UserLogoutAuditEvent>().Information("User {Username} ({SubjectId}) logged out with {Result}", u?.Username, user.GetSubjectId(), "success");
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout) {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                var props = new AuthenticationProperties { RedirectUri = url };
                return SignOut(props, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }
    }
}
