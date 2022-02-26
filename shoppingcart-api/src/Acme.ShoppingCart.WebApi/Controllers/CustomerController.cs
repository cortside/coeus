using System;
using System.Net;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.WebApi.Mappers;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly CustomerModelMapper customerMapper;

        /// <summary>
        /// Initializes a new instance of the WidgetController
        /// </summary>
        public CustomerController(ILogger<CustomerController> logger, ICustomerService service, CustomerModelMapper customerMapper) {
            this.logger = logger;
            this.service = service;
            this.customerMapper = customerMapper;
        }

        /// <summary>
        /// Gets widgets
        /// </summary>
        [HttpGet("")]
        [Authorize(Constants.Authorization.Permissions.GetCustomers)]
        [ProducesResponseType(typeof(PagedList<CustomerModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomersAsync([FromQuery] CustomerSearch search, int pageNumber = 1, int pageSize = 30, string sort = null) {
            var results = await service.SearchCustomersAsync(pageSize, pageNumber, sort ?? "", search).ConfigureAwait(false);
            var models = results.Convert(x => customerMapper.Map(x));
            return Ok(models);
        }

        /// <summary>
        /// Gets a widget by id
        /// </summary>
        /// <param name="id">the id of the widget to get</param>
        [HttpGet("{id}")]
        [ActionName(nameof(GetCustomerAsync))]
        [Authorize(Constants.Authorization.Permissions.GetCustomer)]
        [ProducesResponseType(typeof(CustomerModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomerAsync(Guid id) {
            var dto = await service.GetCustomerAsync(id).ConfigureAwait(false);
            return Ok(customerMapper.Map(dto));
        }

        /// <summary>
        /// Create a new widget
        /// </summary>
        /// <param name="input"></param>
        [HttpPost("")]
        [Authorize(Constants.Authorization.Permissions.CreateCustomer)]
        [ProducesResponseType(typeof(CustomerModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CreateCustomerModel input) {
            var dto = new CustomerDto() {
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email
            };
            var dto2 = await service.CreateCustomerAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetCustomerAsync), new { id = dto.CustomerResourceId }, customerMapper.Map(dto2));
        }

        /// <summary>
        /// Update a widget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        [HttpPut("{id}")]
        [Authorize(Constants.Authorization.Permissions.UpdateCustomer)]
        [ProducesResponseType(typeof(CustomerModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCustomerAsync(Guid id, CreateCustomerModel input) {
            using (LogContext.PushProperty("CustomerResourceId", id)) {
                var dto = new CustomerDto() {
                    CustomerResourceId = id,
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    Email = input.Email
                };

                var result = await service.UpdateCustomerAsync(dto).ConfigureAwait(false);
                return Ok(customerMapper.Map(result));
            }
        }

        /// <summary>
        /// Update a widget
        /// </summary>
        /// <param name="resourceId"></param>
        [HttpPost("{resourceId}/publish")]
        [Authorize(Constants.Authorization.Permissions.UpdateCustomer)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PublishCustomerStateChangedEventAsync(Guid resourceId) {
            using (LogContext.PushProperty("CustomerResourceId", resourceId)) {
                await service.PublishCustomerStateChangedEventAsync(resourceId).ConfigureAwait(false);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }
    }
}
