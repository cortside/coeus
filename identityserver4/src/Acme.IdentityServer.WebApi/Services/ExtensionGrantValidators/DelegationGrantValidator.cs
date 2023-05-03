using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace Acme.IdentityServer.WebApi.Services.ExtensionGrantValidators {
    public class DelegationGrantValidator : IExtensionGrantValidator {
        private readonly ITokenValidator validator;

        private static List<string> NotAllowedClaims => new List<string>
            {
                ClaimTypes.NameIdentifier,
                ClaimTypes.AuthenticationMethod,
                JwtClaimTypes.AccessTokenHash,
                JwtClaimTypes.Audience,
                JwtClaimTypes.AuthenticationMethod,
                JwtClaimTypes.AuthenticationTime,
                JwtClaimTypes.AuthorizedParty,
                JwtClaimTypes.AuthorizationCodeHash,
                JwtClaimTypes.Expiration,
                JwtClaimTypes.IdentityProvider,
                JwtClaimTypes.IssuedAt,
                JwtClaimTypes.Issuer,
                JwtClaimTypes.JwtId,
                JwtClaimTypes.Nonce,
                JwtClaimTypes.NotBefore,
                JwtClaimTypes.ReferenceTokenId,
                JwtClaimTypes.SessionId,
                JwtClaimTypes.Subject,
                JwtClaimTypes.Scope,
                JwtClaimTypes.Confirmation,
            };

        public DelegationGrantValidator(ITokenValidator validator) {
            this.validator = validator;
        }

        public string GrantType => "delegation";

        public async Task ValidateAsync(ExtensionGrantValidationContext context) {
            var userToken = context.Request.Raw.Get("token");

            if (string.IsNullOrEmpty(userToken)) {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            var result = await validator.ValidateAccessTokenAsync(userToken);
            if (result.IsError) {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            // get user's identity
            var claims = new List<Claim>();

            // check to see if token is already a delegation token
            var act = result.Claims.FirstOrDefault(c => c.Type == "act");

            if (act == null) {
                var sub = result.Claims.FirstOrDefault(c => c.Type == "sub").Value;
                foreach (var claim in result.Claims.Where(x => !NotAllowedClaims.Contains(x.Type)).ToList()) {
                    claims.Add(new Claim("act_" + claim.Type, claim.Value));
                }
                claims.Add(new Claim("act", sub));
            } else {
                foreach (var claim in result.Claims.Where(x => x.Type.StartsWith("act_") || x.Type == "act")) {
                    claims.Add(new Claim(claim.Type, claim.Value));
                }
            }

            var subjectId = context.Request.ClientClaims.FirstOrDefault(c => c.Type == "sub").Value;
            context.Result = new GrantValidationResult(subjectId, GrantType, claims);
        }
    }
}
