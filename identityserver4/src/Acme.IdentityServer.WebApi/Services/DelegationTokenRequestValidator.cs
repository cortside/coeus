using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace Acme.IdentityServer.WebApi.Services {

    public class DelegationTokenRequestValidator : ICustomTokenRequestValidator {

        public Task ValidateAsync(CustomTokenRequestValidationContext context) {
            var client = context.Result.ValidatedRequest.Client;

            // add delegation grant_type claim if allowed grant type
            if (client.AllowedGrantTypes.Contains("delegation")) {
                context.Result.ValidatedRequest.ClientClaims.Add(new System.Security.Claims.Claim("grant_type", "delegation"));
            }

            // add the subject_type for client
            context.Result.ValidatedRequest.ClientClaims.Add(new System.Security.Claims.Claim("subject_type", "client"));

            return Task.CompletedTask;
        }
    }
}
