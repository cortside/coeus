using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Acme.IdentityServer.WebApi.Controllers.Client {

    /// <summary>
    /// Clients endpoints controller
    /// </summary>
    [Route("api/clients")]
    public class ClientController : ControllerBase {

        private readonly IClientService clientService;

        public ClientController(IClientService clientService) {
            this.clientService = clientService;
        }

        /// <summary>
        /// Insert or update a client by client ID (readable string name, not data store identifier)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{clientId}")]
        [EnableCors("AllowAll")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Update(string clientId, [FromBody] UpdateClientRequest updateClientRequest) {

            if (updateClientRequest.GrantType != ClientConstants.GrantTypes.Implicit) {
                return BadRequest();
            }

            var upsertedClient = clientService.UpdateClient(clientId, updateClientRequest);
            return Ok(upsertedClient);
        }

        /// <summary>
        /// Delete a client by its client ID (readable string name, not data store identifier)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{clientId}")]
        [EnableCors("AllowAll")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Delete(string clientId) {
            clientService.DeleteClient(clientId);
            return Ok();
        }
    }
}
