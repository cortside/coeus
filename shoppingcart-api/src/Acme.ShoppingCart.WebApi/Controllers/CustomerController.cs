using System;
using System.Net;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.WebApi.Mappers;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Cortside.Common.Messages.MessageExceptions;
using Cortside.Common.Messages.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Acme.ShoppingCart.WebApi.Controllers {
    /// <summary>
    /// Represents the shared functionality/resources of the customer resource
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
        private readonly IEncryptionService encryptionService;

        /// <summary>
        /// Initializes a new instance of the CustomerController
        /// </summary>
        public CustomerController(ILogger<CustomerController> logger, ICustomerService service, CustomerModelMapper customerMapper, IEncryptionService encryptionService) {
            this.logger = logger;
            this.service = service;
            this.customerMapper = customerMapper;
            this.encryptionService = encryptionService;
        }

        /// <summary>
        /// Gets customers
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
        /// Gets customers by post
        /// </summary>
        [HttpPost("search")]
        [Authorize(Constants.Authorization.Permissions.GetCustomers)]
        [ProducesResponseType(typeof(PagedList<CustomerModel>), StatusCodes.Status200OK)]
        public IActionResult GetCustomersByPost([FromBody] CustomerSearchModel search) {
            if (search == null) {
                return BadRequest(new ErrorModel { Message = "Search model cannot be null", Type = nameof(BadRequestResponseException) });
            }

            string encryptedString = encryptionService.EncryptString(search);
            string reqUrl = HttpHelper.BuildUriFromRequest(Request);
            reqUrl += "?encryptedParams=" + encryptedString;

            Response.Headers.Add("Location", reqUrl);
            return StatusCode((int)HttpStatusCode.SeeOther);
        }

        /// <summary>
        /// Returns search results for rebate requests
        /// </summary>
        /// <param name="encryptedParams"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [Authorize(Constants.Authorization.Permissions.GetCustomers)]
        [ProducesResponseType(typeof(PagedList<CustomerModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCustomersBySearch(string encryptedParams) {
            if (string.IsNullOrWhiteSpace(encryptedParams)) {
                return BadRequest(new ErrorModel { Message = "Encrypted params cannot be null, empty or whitespace", Type = nameof(BadRequestResponseException) });
            }

            var param = encryptionService.DecryptString<CustomerSearchModel>(encryptedParams);
            var search = new CustomerSearch() {
                CustomerResourceId = param.CustomerResourceId,
                FirstName = param.FirstName,
                LastName = param.LastName
            };

            var results = await service.SearchCustomersAsync(param.PageSize, param.PageNumber, param.Sort, search).ConfigureAwait(false);
            var models = results.Convert(x => customerMapper.Map(x));
            return Ok(models);
        }

        /// <summary>
        /// Gets a customer by id
        /// </summary>
        /// <param name="id">the id of the customer to get</param>
        [HttpGet("{id}")]
        [ActionName(nameof(GetCustomerAsync))]
        [Authorize(Constants.Authorization.Permissions.GetCustomer)]
        [ProducesResponseType(typeof(CustomerModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomerAsync(Guid id) {
            var dto = await service.GetCustomerAsync(id).ConfigureAwait(false);
            return Ok(customerMapper.Map(dto));
        }

        /// <summary>
        /// Create a new customer
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
        /// Update a customer
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
        /// Update a customer
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
