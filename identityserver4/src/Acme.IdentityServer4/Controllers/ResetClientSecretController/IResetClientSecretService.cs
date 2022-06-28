using System;
using Cortside.IdentityServer.Controllers.ResetClientSecret;

namespace Cortside.IdentityServer.WebApi.Controllers.ResetClientSecretController {
    public interface IResetClientSecretService {
        SecretKeyModel BuildSecretKeyViewModel(Guid requestId);
        VerifyIdentityModel BuildVerifyIdentityViewModel(string encodedRequestId);
        VerifyIdentityModel BuildVerifyIdentityViewModel(Guid requestId);
    }
}
