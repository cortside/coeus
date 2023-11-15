using System;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Acme.IdentityServer.WebApi.Controllers.ClientSecretRequests {

    [Route("api/v1/clientsecretrequests")]
    public class ClientSecretRequestsController : Controller {
        private readonly IClientSecretService clientSecretService;

        public ClientSecretRequestsController(IClientSecretService clientSecretService) {
            this.clientSecretService = clientSecretService;
        }

        [HttpPost]
        [Route("{clientSecretRequestId}/SendVerificationCode")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> SendVerificationCode(Guid clientSecretRequestId, [FromBody] SendVerificationCodeModel sendVerificationCodeModel) {
            if (clientSecretRequestId == Guid.Empty) {
                return BadRequest("ClientSecretRequestId cannot be null or empty.");
            }

            if (sendVerificationCodeModel == null || string.IsNullOrWhiteSpace(sendVerificationCodeModel.TokenHash)) {
                return BadRequest("SendVerificationCodeModel cannot be null or empty.");
            }

            await clientSecretService.SendVerificationCode(clientSecretRequestId, sendVerificationCodeModel);
            return Ok();
        }
    }
}
