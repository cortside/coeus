using System;
using System.Threading.Tasks;
using Cortside.Common.Messages;
using Acme.IdentityServer.WebApi.Assemblers;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Exceptions;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Acme.IdentityServer.WebApi.Controllers {
    /// <summary>
    /// Users endpoints controller
    /// </summary>
    [Route("api/users")]
    [ApiController]
    [ApiVersionNeutral]
    [Produces("application/json")]
    public class UserController : ControllerBase {
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;
        private readonly IUserModelAssembler userModelAssembler;
        private readonly IClientService clientService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            IUserModelAssembler userModelAssembler,
            IClientService clientService
            ) {
            this.logger = logger;
            this.userService = userService;
            this.userModelAssembler = userModelAssembler;
            this.clientService = clientService;
        }

        /// <summary>
        /// Get a user by its subject id
        /// </summary>
        [HttpGet]
        [Route("{id:guid}", Name = "GetUser")]
        [EnableCors("AllowAll")]
        [ProducesResponseType(typeof(Models.Output.UserOutputModel), StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetUserAsync(Guid id) {
            logger.LogInformation($"Received GetUser request for {id}");
            var user = await userService.FindBySubjectIdAsync(id).ConfigureAwait(false);
            if (user != null) {
                return Ok(userModelAssembler.ToUserOutputModel(user));
            }
            logger.LogInformation($"User {id} not found");
            var client = clientService.GetClient(id.ToString());
            if (client != null) {
                return Ok(userModelAssembler.ToUserOutputModel(client));
            }
            logger.LogInformation($"Client with sub {id} not found");
            return NotFound();
        }

        /// <summary>
        /// Add a user
        /// </summary>
        /// <remarks>
        /// On User Create
        /// * Will require authenticated caller(client would need the identity scope)
        ///     This API service will validate the token
        ///     User can't authenticate if inactive
        /// * Will persist new user in Identity Server database
        /// * Will generate a new unique userId
        /// * Will use the authenticated subject for creation user if not provided
        /// * Will use Active for status
        /// 
        /// On Return
        /// * Will return newly created user representation on successful response
        /// * Will return 201 when successfully created
        ///     The return body will contain the newly created user object with claims
        ///     A Location header should be returned with the location of a GET url
        /// * Will return Location header when successfully created
        /// 
        /// On Failure
        /// * Will return 400 if required values are missing. 
        ///     Any other values not explicitly provided should be defaulted
        /// * Will return 400 if the user name already exists. 
        ///     Note: Inactive users are considered "existing"
        /// </remarks>
        [HttpPost]
        [EnableCors("AllowAll")]
        [ProducesResponseType(typeof(Models.Output.UserOutputModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AddUser([FromBody] Models.Input.CreateUserModel userRequest) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            User user = null;
            try {
                user = await userService.CreateUser(userRequest);
            } catch (MessageException err) {
                logger.LogCritical(err, $"Failed to add user ({userRequest.Username})");
                return BadRequest();
            }

            var model = userModelAssembler.ToUserOutputModel(user);
            return CreatedAtRoute("GetUser", new { id = user.UserId }, model);
        }

        /// <summary>
        /// Update a user by user ID (data store GUID identifier)
        /// </summary>
        /// <param name="id">User ID to update</param>
        /// <param name="userRequest">The user settings to update</param>
        /// <remarks>
        /// On User Update
        /// * Will require authenticated caller, scope should be identity
        ///     This API service will validate the token
        ///     User can't authenticate if inactive
        /// * Will update existing claim set to matched passed in set
        /// * Will persist updated existing user
        /// * Will use the authenticated subject for last modified user if not provided
        /// 
        /// On Return
        /// * Will return the updated user representation on successful response
        /// * Will return 200 when successfully created
        /// * Will return Location header when successfully created
        /// 
        /// On Error
        /// * Will return 400 if required values are missing
        /// * Will return 404 if a user is not found. 
        ///     Note: Inactive users are considered "not found" (deleted)
        /// </remarks>
        [HttpPut]
        [Route("{id:guid}")]
        [EnableCors("AllowAll")]
        [ProducesResponseType(typeof(Models.Output.UserOutputModel), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] Models.Input.UpdateUserModel userRequest) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            User user = null;
            try {
                user = await userService.UpdateUser(id, userRequest);
            } catch (ResourceNotFoundMessage) {
                return NotFound();
            }

            var model = userModelAssembler.ToUserOutputModel(user);

            string location = $"{Request.Scheme}://{Request.Host}/api/users/{id:d}";
            Response.Headers.Add("Location", location);

            return Ok(model);
        }

        /// <summary>
        /// Delete a user by its user ID (data store GUID identifier)
        /// </summary>
        /// <remarks>
        /// On User Delete
        /// * Will require authenticated caller, scope should be identity
        ///     This API service will validate the token
        ///     User can't authenticate if inactive
        /// * Will persist existing user as Inactive
        /// * Will use the authenticated subject for last modified user if not provided
        /// 
        /// On Return
        /// * Will return 204 when successfully updated
        /// 
        /// On Error
        /// * Will return 404 if user is not found
        ///     Note: Inactive users are considered "not found" (deleted)
        /// </remarks>
        [HttpDelete]
        [Route("{id}")]
        [EnableCors("AllowAll")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult DeactivateUser(Guid id) {
            try {
                userService.DeactivateUser(id);
            } catch (ResourceNotFoundMessage) {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Updates a user's password
        /// </summary>
        /// <param name="id">User subject id</param>
        /// <param name="model">Password</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}/password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult UpdatePassword(Guid id, [FromBody] UpdatePasswordModel model) {
            if (!ModelState.IsValid) { return BadRequest(); }

            try {
                userService.UpdatePassword(id, model);
            } catch (ResourceNotFoundMessage) {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Updates a user's password
        /// </summary>
        /// <param name="id">User subject id</param>
        /// <param name="model">Lock</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}/lock")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult UpdateUserLock(Guid id, [FromBody] UpdateLockModel model) {
            if (!ModelState.IsValid) { return BadRequest(); }
            User user = null;
            try {
                user = userService.UpdateUserLock(id, model);
            } catch (ResourceNotFoundMessage) {
                return NotFound();
            }

            var outoutModel = userModelAssembler.ToUserOutputModel(user);

            string location = $"{Request.Scheme}://{Request.Host}/api/users/{id:d}";
            Response.Headers.Add("Location", location);

            return Ok(outoutModel);
        }
    }
}
