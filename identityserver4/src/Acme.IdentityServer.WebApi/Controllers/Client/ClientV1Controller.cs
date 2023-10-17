using System;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.IdentityServer.WebApi.Controllers.Client {

    /// <summary>
    /// Clients v1 endpoints controller
    /// </summary>
    [Route("api/v1/clients")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class ClientV1Controller : ControllerBase {
        private readonly IClientService _clientService;
        private readonly IClientSecretService _clientSecretService;

        public ClientV1Controller(IClientService clientService, IClientSecretService clientSecretService) {
            _clientService = clientService;
            _clientSecretService = clientSecretService;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id) {
            if (id == 0) {
                return BadRequest("Id cannot be null or empty");
            }

            var client = _clientService.GetClient(id);
            return Ok(client);
        }

        /// <summary>
        /// Creates a new client with the provided info
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Client Model</returns>
        [HttpPost]
        public IActionResult Create([FromBody] CreateClientModel createClientRequest) {

            if (string.IsNullOrEmpty(createClientRequest.ClientId)) {
                return BadRequest("ClientId cannot be null or empty");
            }

            if (string.IsNullOrEmpty(createClientRequest.SubjectId)) {
                return BadRequest("SubjectId cannot be null or empty");
            }

            if (string.IsNullOrEmpty(createClientRequest.Email)) {
                return BadRequest("Email cannot be null or empty");
            }

            if (string.IsNullOrEmpty(createClientRequest.PhoneNumber)) {
                return BadRequest("PhoneNumber cannot be null or empty");
            }
            try {
                var client = _clientService.CreateClient(createClientRequest);
                return Ok(client);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// Endpoint to update client
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateClientModel request) {

            if (id == 0) {
                return BadRequest("Id cannot be null or empty");
            }

            if (request == null) {
                return BadRequest("UpdateClientModel cannot be null");
            }
            try {
                var client = _clientService.UpdateClient(id, request);
                return Ok(client);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Endpoint to reset client secret
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/resetsecret")]
        [AllowAnonymous]
        public IActionResult ResetSecret(int id) {

            if (id == 0) {
                return BadRequest("Id cannot be null or empty");
            }

            try {
                _clientSecretService.ResetSecret(id);
                return Ok();
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Endpoint to update client scopes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}/scopes")]
        public IActionResult UpdateScopes(int id, [FromBody] UpdateClientScopesModel request) {

            if (id == 0) {
                return BadRequest("Id cannot be null or empty");
            }

            if (request == null) {
                return BadRequest("UpdateClientScopesModel cannot be null");
            }

            var response = _clientService.UpdateClientScopes(id, request);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update client claims
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}/claims")]
        public IActionResult UpdateClaims(int id, [FromBody] UpdateClientClaimsModel request) {

            if (id == 0) {
                return BadRequest("Id cannot be null or empty");
            }

            if (request == null) {
                return BadRequest("UpdateClientClaimsModel cannot be null");
            }

            var response = _clientService.UpdateClientClaims(id, request);
            return Ok(response);
        }

        [HttpGet("subjectId={subjectId}")]
        public IActionResult GetClientBySubjectId(string subjectId) {
            if (string.IsNullOrEmpty(subjectId)) {
                return BadRequest("Id cannot be null or empty");
            }

            Data.Client response = _clientService.GetClient(subjectId);
            if (response == null) {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
