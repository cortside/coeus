using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Acme.IdentityServer.WebApi.Services.ExtensionGrantValidators {
    public class RecaptchaGrantValidator : IExtensionGrantValidator {
        private readonly ILogger<RecaptchaGrantValidator> logger;
        private readonly IGoogleRecaptchaV3Service service;
        private readonly IConfiguration configuration;

        public RecaptchaGrantValidator(ILogger<RecaptchaGrantValidator> logger, IGoogleRecaptchaV3Service service, IConfiguration configuration) {
            this.logger = logger;
            this.service = service;
            this.configuration = configuration;
        }

        public string GrantType => "recaptcha";

        public async Task ValidateAsync(ExtensionGrantValidationContext context) {
            var secret = context.Request.Raw.Get("site_secret");
            var token = context.Request.Raw.Get("recaptcha_token");
            var remoteip = context.Request.Raw.Get("remote_ip");
            var referenceId = context.Request.Raw.Get("resource_id");
            var client = context.Request.Raw.Get("client");
            var version = context.Request.Raw.Get("version");

            logger.LogDebug($"Getting grant for recaptcha token {token} from remoteip {remoteip}");

            if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(token)) {
                logger.LogInformation($"Missing require value for secret or token");
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            GRequestModel rm = new GRequestModel(token, remoteip);
            rm.secret = secret;
            rm.path = configuration["GoogleRecaptchaV3:ApiUrl"];
            service.InitializeRequest(rm);

            if (!await service.Execute()) {
                logger.LogInformation($"Failed to validate recaptcha token with error codes {string.Join(",", service.Response.error_codes)}");
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            // clear the claims from the client making the call
            context.Request.ClientClaims.Clear();

            //TODO: this is the "anonymous" guid
            var subjectId = "00000000-0000-0000-0000-000000000001";

            var claims = new List<Claim>() {
                new Claim("sub", subjectId),
                new Claim("hostname", service.Response.hostname),
                new Claim("score", service.Response.score.ToString()),
                new Claim("action", service.Response.action),
            };
            if (!string.IsNullOrWhiteSpace(referenceId)) {
                claims.Add(new Claim("resource_id", referenceId));
            }
            if (!string.IsNullOrWhiteSpace(client)) {
                claims.Add(new Claim("client", client));
            }
            if (!string.IsNullOrWhiteSpace(version)) {
                claims.Add(new Claim("version", version));
            }

            context.Result = new GrantValidationResult(subjectId, GrantType, claims);

            logger.LogInformation($"Successfully created grant for action {service.Response.action} with score {service.Response.score}");
        }
    }
}
