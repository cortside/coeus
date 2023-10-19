using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.IdentityServer.WebApi.Controllers.ResetClientSecretController {
    public class VerifyIdentityModel {
        public Guid RequestId { get; set; }

        public string TokenHash { get; set; }

        [Required(ErrorMessage = "Verification Code is required.")]
        [MinLength(6, ErrorMessage = "Verification Code needs to have six digits.")]
        public string VerificationCode { get; set; }

        public string Last4PhoneNumber { get; set; }
    }
}
