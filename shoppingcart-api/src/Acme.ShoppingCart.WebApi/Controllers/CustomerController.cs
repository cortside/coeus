using System;
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
        [Authorize(Constants.Authorization.Permissions.GetCustomers)]
        [ProducesResponseType(typeof(ShoppingCart.Data.Paging.PagedList<CustomerDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCustomersAsync(int pageNumber = 1, int pageSize = 30, string sort = null) {
            var results = await service.SearchCustomersAsync(pageSize, pageNumber, sort ?? "").ConfigureAwait(false);
            return Ok(results);
        }

        /// <summary>
        /// Gets a widget by id
        /// </summary>
        /// <param name="id">the id of the widget to get</param>
        [HttpGet("{id}")]
        [ActionName(nameof(GetCustomerAsync))]
        [Authorize(Constants.Authorization.Permissions.GetCustomer)]
        [ProducesResponseType(typeof(CustomerDto), 200)]
        public async Task<IActionResult> GetCustomerAsync(Guid id) {
            var widget = await service.GetCustomerAsync(id).ConfigureAwait(false);
            return Ok(widget);
        }

        /// <summary>
        /// Create a new widget
        /// </summary>
        /// <param name="input"></param>
        [HttpPost("")]
        [Authorize(Constants.Authorization.Permissions.CreateCustomer)]
        [ProducesResponseType(typeof(CustomerDto), 201)]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerRequest input) {
            var dto = new CustomerDto() {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email
            };
            var widget = await service.CreateCustomerAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetCustomerAsync), new { id = widget.CustomerId }, widget);
        }

        /// <summary>
        /// Update a widget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        [HttpPut("{id}")]
        [Authorize(Constants.Authorization.Permissions.UpdateCustomer)]
        [ProducesResponseType(typeof(CustomerDto), 204)]
        public async Task<IActionResult> UpdateCustomerAsync(int id, CustomerRequest input) {
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
        /// <param name="resourceId"></param>
        /// <param name="id"></param>
        [HttpPost("{id}/publish")]
        [Authorize(Constants.Authorization.Permissions.UpdateCustomer)]
        [ProducesResponseType(typeof(CustomerDto), 204)]
        public async Task<IActionResult> PublishCustomerStateChangedEventAsync(Guid resourceId) {
            using (LogContext.PushProperty("CustomerResourceId", resourceId)) {
                await service.PublishCustomerStateChangedEventAsync(resourceId).ConfigureAwait(false);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }
    }
}
