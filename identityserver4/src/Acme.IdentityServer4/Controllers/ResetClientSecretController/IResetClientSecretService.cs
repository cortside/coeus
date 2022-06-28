using System;
using EnerBank.IdentityServer.Controllers.ResetClientSecret;

namespace EnerBank.IdentityServer.WebApi.Controllers.ResetClientSecretController {
    public interface IResetClientSecretService {
        SecretKeyModel BuildSecretKeyViewModel(Guid requestId);
        VerifyIdentityModel BuildVerifyIdentityViewModel(string encodedRequestId);
        VerifyIdentityModel BuildVerifyIdentityViewModel(Guid requestId);
    }
}
