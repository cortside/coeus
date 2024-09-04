using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Acme.ShoppingCart.Facade;
using Acme.ShoppingCart.WebApi.Mappers;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Asp.Versioning;
using Cortside.AspNetCore;
using Cortside.AspNetCore.Common.Paging;
using Cortside.AspNetCore.Filters.Models;
using Cortside.Common.Cryptography;
using Cortside.Common.Messages.MessageExceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class CustomerController : ControllerBase {
        private readonly ICustomerFacade facade;
        private readonly CustomerModelMapper customerMapper;
        private readonly IEncryptionService encryptionService;

        /// <summary>
        /// Initializes a new instance of the CustomerController
        /// </summary>
        public CustomerController(ICustomerFacade facade, CustomerModelMapper customerMapper, IEncryptionService encryptionService) {
            this.facade = facade;
            this.customerMapper = customerMapper;
            this.encryptionService = encryptionService;
        }

        /// <summary>
        /// Gets customers
        /// </summary>
        [HttpGet("")]
        [Authorize(Constants.Authorization.Permissions.GetCustomers)]
        [ProducesResponseType(typeof(PagedList<CustomerModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomersAsync([FromQuery] CustomerSearchModel search) {
            var searchDto = customerMapper.MapToDto(search);
            var results = await facade.SearchCustomersAsync(searchDto).ConfigureAwait(false);
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

            string encryptedString = encryptionService.EncryptObject(search);
            string reqUrl = HttpHelper.BuildUriFromRequest(Request);
            reqUrl += "?encryptedParams=" + HttpUtility.UrlEncode(encryptedString);

            Response.Headers.Append("Location", reqUrl);
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
        public async Task<IActionResult> GetCustomersBySearchAsync(string encryptedParams) {
            if (string.IsNullOrWhiteSpace(encryptedParams)) {
                return BadRequest(new ErrorModel { Message = "Encrypted params cannot be null, empty or whitespace", Type = nameof(BadRequestResponseException) });
            }

            var param = encryptionService.DecryptObject<CustomerSearchModel>(encryptedParams);
            var search = customerMapper.MapToDto(param);

            var results = await facade.SearchCustomersAsync(search).ConfigureAwait(false);
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
            var dto = await facade.GetCustomerAsync(id).ConfigureAwait(false);
            return Ok(customerMapper.Map(dto));
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="input"></param>
        [HttpPost("")]
        [Authorize(Constants.Authorization.Permissions.CreateCustomer)]
        [ProducesResponseType(typeof(CustomerModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] UpdateCustomerModel input) {
            var inputDto = customerMapper.MapToDto(input);
            var dto = await facade.CreateCustomerAsync(inputDto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetCustomerAsync), new { id = dto.CustomerResourceId }, customerMapper.Map(dto));
        }

        /// <summary>
        /// Update a customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        [HttpPut("{id}")]
        [Authorize(Constants.Authorization.Permissions.UpdateCustomer)]
        [ProducesResponseType(typeof(CustomerModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCustomerAsync(Guid id, UpdateCustomerModel input) {
            using (LogContext.PushProperty("CustomerResourceId", id)) {
                var dto = customerMapper.MapToDto(input);

                var result = await facade.UpdateCustomerAsync(id, dto).ConfigureAwait(false);
                return Ok(customerMapper.Map(result));
            }
        }

        /// <summary>
        /// Update a customer
        /// </summary>
        /// <param name="resourceId"></param>
        [HttpPost("{resourceId}/publish")]
        [Authorize(Constants.Authorization.Permissions.PublishCustomer)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PublishCustomerStateChangedEventAsync(Guid resourceId) {
            using (LogContext.PushProperty("CustomerResourceId", resourceId)) {
                await facade.PublishCustomerStateChangedEventAsync(resourceId).ConfigureAwait(false);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }
    }
}
