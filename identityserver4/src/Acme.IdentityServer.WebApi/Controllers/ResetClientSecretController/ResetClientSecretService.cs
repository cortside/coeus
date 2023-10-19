using System;
using Acme.IdentityServer.WebApi.Services;

namespace Acme.IdentityServer.WebApi.Controllers.ResetClientSecretController {
    public class ResetClientSecretService : IResetClientSecretService {
        private readonly IClientSecretService clientSecretService;

        public ResetClientSecretService(IClientSecretService clientSecretService) {
            this.clientSecretService = clientSecretService;
        }

        public VerifyIdentityModel BuildVerifyIdentityViewModel(string encodedRequestId) {
            var vm = new VerifyIdentityModel();

            var validationResponse = clientSecretService.ValidateClientSecretRequest(encodedRequestId);
            if (!validationResponse.IsValid) {
                throw new InvalidOperationException("Request is invalid.");
            }

            vm.RequestId = validationResponse.DecodedOutput.RequestId;
            vm.Last4PhoneNumber = validationResponse.Last4PhoneNumber;
            vm.TokenHash = validationResponse.DecodedOutput.TokenHash;

            return vm;
        }

        public VerifyIdentityModel BuildVerifyIdentityViewModel(Guid requestId) {
            var vm = new VerifyIdentityModel();

            var phoneNumber = clientSecretService.GetClientPhoneNumberFromSecretRequestId(requestId);

            vm.RequestId = requestId;
            vm.Last4PhoneNumber = phoneNumber.Substring(phoneNumber.Length - 4);

            return vm;
        }

        public SecretKeyModel BuildSecretKeyViewModel(Guid requestId) {
            var vm = new SecretKeyModel();

            vm.SecretKey = clientSecretService.GetClientSecretKey(requestId);

            return vm;
        }
    }
}
