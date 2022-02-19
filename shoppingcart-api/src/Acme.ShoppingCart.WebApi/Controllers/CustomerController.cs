using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Acme.ShoppingCart.WebApi.Controllers {
    /// <summary>
    /// Represents the shared functionality/resources of the widget resource
    /// </summary>
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Produces("application/json")]
    [ApiController]
    [Route("api/v{version:apiVersion}/customers")]
    public class CustomerController : Controller {
        private readonly ILogger logger;
        private readonly ICustomerService service;

        /// <summary>
        /// Initializes a new instance of the WidgetController
        /// </summary>
        public CustomerController(ILogger<CustomerController> logger, ICustomerService service) {
            this.logger = logger;
            this.service = service;
        }

        /// <summary>
        /// Gets widgets
        /// </summary>
        [HttpGet("")]
        [Authorize(Constants.Authorization.Permissions.GetWidgets)]
        [ProducesResponseType(typeof(List<CustomerDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetWidgetsAsync() {
            var widgets = await service.GetCustomersAsync().ConfigureAwait(false);
            return Ok(widgets);
        }

        /// <summary>
        /// Gets a widget by id
        /// </summary>
        /// <param name="id">the id of the widget to get</param>
        [HttpGet("{id}")]
        [ActionName(nameof(GetWidgetAsync))]
        [Authorize(Constants.Authorization.Permissions.GetWidget)]
        [ProducesResponseType(typeof(CustomerDto), 200)]
        public async Task<IActionResult> GetWidgetAsync(Guid id) {
            var widget = await service.GetCustomerAsync(id).ConfigureAwait(false);
            return Ok(widget);
        }

        /// <summary>
        /// Create a new widget
        /// </summary>
        /// <param name="input"></param>
        [HttpPost("")]
        [Authorize(Constants.Authorization.Permissions.CreateWidget)]
        [ProducesResponseType(typeof(CustomerDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateWidgetAsync([FromBody] CustomerRequest input) {
            var dto = new CustomerDto() {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email
            };
            var widget = await service.CreateCustomerAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetWidgetAsync), new { id = widget.CustomerId }, widget);
        }

        /// <summary>
        /// Update a widget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        [HttpPut("{id}")]
        [Authorize(Constants.Authorization.Permissions.UpdateWidget)]
        [ProducesResponseType(typeof(CustomerDto), 204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateWidgetAsync(int id, CustomerRequest input) {
            using (LogContext.PushProperty("WidgetId", id)) {
                var dto = new CustomerDto() {
                    CustomerId = id,
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    Email = input.Email
                };

                var widget = await service.UpdateCustomerAsync(dto).ConfigureAwait(false);
                return StatusCode((int)HttpStatusCode.NoContent, widget);
            }
        }

        /// <summary>
        /// Update a widget
        /// </summary>
        /// <param name="id"></param>
        [HttpPost("{id}/publish")]
        [Authorize(Constants.Authorization.Permissions.UpdateWidget)]
        [ProducesResponseType(typeof(CustomerDto), 204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PublishWidgetStateChangedEventAsync(int id) {
            using (LogContext.PushProperty("WidgetId", id)) {
                await service.PublishCustomerStateChangedEventAsync(id).ConfigureAwait(false);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }
    }
}
