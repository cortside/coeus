using System;
using Acme.IdentityServer.Controllers.ResetClientSecret;

namespace Acme.IdentityServer.WebApi.Controllers.ResetClientSecretController {
    public interface IResetClientSecretService {
        SecretKeyModel BuildSecretKeyViewModel(Guid requestId);
        VerifyIdentityModel BuildVerifyIdentityViewModel(string encodedRequestId);
        VerifyIdentityModel BuildVerifyIdentityViewModel(Guid requestId);
    }
}
