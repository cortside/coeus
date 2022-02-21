using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.WebApi.Controllers {
    /// <summary>
    /// Represents the shared functionality/resources of the widget resource
    /// </summary>
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Produces("application/json")]
    [ApiController]
    [Route("api/v{version:apiVersion}/orders")]
    public class OrderController : Controller {
        private readonly ILogger logger;
        private readonly IOrderService service;

        /// <summary>
        /// Initializes a new instance of the WidgetController
        /// </summary>
        public OrderController(ILogger<OrderController> logger, IOrderService service) {
            this.logger = logger;
            this.service = service;
        }

        /// <summary>
        /// Gets orders
        /// </summary>
        [HttpGet("")]
        [Authorize(Constants.Authorization.Permissions.GetOrders)]
        [ProducesResponseType(typeof(List<OrderDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersAsync([FromQuery] OrderSearch search, int pageNumber = 1, int pageSize = 30, string sort = null) {
            var results = await service.SearchOrdersAsync(pageSize, pageNumber, sort ?? "", search).ConfigureAwait(false);
            return Ok(results);
        }

        /// <summary>
        /// Gets a widget by id
        /// </summary>
        /// <param name="id">the id of the widget to get</param>
        [HttpGet("{id}")]
        [ActionName(nameof(GetOrderAsync))]
        [Authorize(Constants.Authorization.Permissions.GetOrder)]
        [ProducesResponseType(typeof(OrderDto), 200)]
        public async Task<IActionResult> GetOrderAsync(Guid id) {
            var widget = await service.GetOrderAsync(id).ConfigureAwait(false);
            return Ok(widget);
        }

        /// <summary>
        /// Create a new widget
        /// </summary>
        /// <param name="input"></param>
        [HttpPost("")]
        [Authorize(Constants.Authorization.Permissions.CreateOrder)]
        [ProducesResponseType(typeof(OrderDto), 201)]
        public async Task<IActionResult> CreateOrderAsync([FromBody] OrderRequest input) {
            var dto = new OrderDto() {
                Customer = new CustomerDto() {
                    CustomerResourceId = input.CustomerResourceId,
                },
                Address = new AddressDto() {
                    Street = input.Address.Street,
                    City = input.Address.City,
                    State = input.Address.State,
                    Country = input.Address.Country,
                    ZipCode = input.Address.ZipCode
                },
                Items = new System.Collections.Generic.List<OrderItemDto>()
            };
            foreach (var item in input.Items) {
                dto.Items.Add(new OrderItemDto() { Sku = item.Sku, Quantity = item.Quantity });
            }

            var order = await service.CreateOrderAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetOrderAsync), new { id = order.OrderResourceId }, order);
        }

        ///// <summary>
        ///// Update a widget
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="input"></param>
        //[HttpPut("{id}")]
        //[Authorize(Constants.Authorization.Permissions.UpdateWidget)]
        //[ProducesResponseType(typeof(OrderDto), 204)]
        //public async Task<IActionResult> UpdateWidgetAsync(int id, CustomerRequest input) {
        //    using (LogContext.PushProperty("WidgetId", id)) {
        //        var dto = new CustomerDto() {
        //            CustomerId = id,
        //            FirstName = input.FirstName,
        //            LastName = input.LastName,
        //            Email = input.Email
        //        };

        //        var widget = await service.UpdateCustomerAsync(dto).ConfigureAwait(false);
        //        return StatusCode((int)HttpStatusCode.NoContent, widget);
        //    }
        //}

        ///// <summary>
        ///// Update a widget
        ///// </summary>
        ///// <param name="id"></param>
        //[HttpPost("{id}/publish")]
        //[Authorize(Constants.Authorization.Permissions.UpdateWidget)]
        //[ProducesResponseType(typeof(OrderDto), 204)]
        //[ProducesResponseType(400)]
        //public async Task<IActionResult> PublishWidgetStateChangedEventAsync(int id) {
        //    using (LogContext.PushProperty("WidgetId", id)) {
        //        await service.PublishCustomerStateChangedEventAsync(id).ConfigureAwait(false);
        //        return StatusCode((int)HttpStatusCode.NoContent);
        //    }
        //}
    }
}
