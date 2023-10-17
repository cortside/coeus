using System;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Acme.IdentityServer.WebApi.Controllers.ResetClientSecretController {
    public class ResetClientSecretController : Controller {
        private readonly IClientSecretService clientSecretService;
        private readonly IResetClientSecretService resetClientSecretService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ResetClientSecretController(IClientSecretService clientSecretService, IResetClientSecretService resetClientSecretService, IHttpContextAccessor httpContextAccessor) {
            this.clientSecretService = clientSecretService;
            this.resetClientSecretService = resetClientSecretService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            httpContextAccessor.HttpContext.Request.Query.TryGetValue("requestId", out var stringValues);

            if (stringValues.Count == 0) {
                return View("InvalidLink");
            }

            var requestId = stringValues.ToString();

            VerifyIdentityModel vm = null;

            try {
                vm = resetClientSecretService.BuildVerifyIdentityViewModel(requestId);
                await clientSecretService.SendVerificationCode(vm.RequestId, new SendVerificationCodeModel { TokenHash = vm.TokenHash });
            } catch {
                return View("InvalidLink");
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from verification code submitq
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyIdentity(string requestId, string verificationCode, string tokenHash, string button) {
            if (string.IsNullOrEmpty(verificationCode)) {
                ModelState.AddModelError("submiterror", "Verification Code is required.");
            }

            if (verificationCode?.Length != 6) {
                ModelState.AddModelError("submiterror", "Verification Code should be 6 digits.");
            }

            var vm = resetClientSecretService.BuildVerifyIdentityViewModel(Guid.Parse(requestId));
            vm.TokenHash = tokenHash;

            if (ModelState.IsValid) {
                var response = clientSecretService.IsVerificationCodeValid(vm.RequestId, verificationCode);
                if (!response.IsValid) {
                    ModelState.AddModelError("submiterror", $"{response.Reason}");
                }
            }

            if (ModelState.IsValid) {
                return SecretKey(requestId);
            }

            return View("Index", vm);
        }

        /// <summary>
        /// Handle postback from resend code link
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ResendCode(string requestId, string tokenHash) {
            await clientSecretService.SendVerificationCode(Guid.Parse(requestId), new SendVerificationCodeModel { TokenHash = tokenHash });

            return NoContent();
        }

        /// <summary>
        /// Handle postback from verification code submit
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SecretKey(string requestId) {
            var vm = resetClientSecretService.BuildSecretKeyViewModel(Guid.Parse(requestId));

            return View("SecretKey", vm);
        }
    }
}
