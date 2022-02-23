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
using Serilog.Context;

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
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderModel input) {
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
            foreach (var item in input.Items ?? new List<CreateOrderItemModel>()) {
                dto.Items.Add(new OrderItemDto() { Sku = item.Sku, Quantity = item.Quantity });
            }

            var order = await service.CreateOrderAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetOrderAsync), new { id = order.OrderResourceId }, order);
        }

        /// <summary>
        /// Update a widget
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        [HttpPut("{id}")]
        [Authorize(Constants.Authorization.Permissions.UpdateOrder)]
        [ProducesResponseType(typeof(CustomerDto), 200)]
        public async Task<IActionResult> UpdateOrderAsync(Guid id, CreateOrderModel input) {
            using (LogContext.PushProperty("OrderResourceId", id)) {
                var dto = new OrderDto() {
                    OrderResourceId = id
                    //FirstName = input.FirstName,
                    //LastName = input.LastName,
                    //Email = input.Email
                };

                var result = await service.UpdateOrderAsync(dto).ConfigureAwait(false);
                return Ok(result);
            }
        }

        /// <summary>
        /// Update a widget
        /// </summary>
        /// <param name="resourceId"></param>
        [HttpPost("{resourceId}/publish")]
        [Authorize(Constants.Authorization.Permissions.UpdateCustomer)]
        [ProducesResponseType(typeof(CustomerDto), 204)]
        public async Task<IActionResult> PublishCustomerStateChangedEventAsync(Guid resourceId) {
            using (LogContext.PushProperty("OrderResourceId", resourceId)) {
                await service.PublishOrderStateChangedEventAsync(resourceId).ConfigureAwait(false);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// Add an order item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        [HttpPut("{id}")]
        [Authorize(Constants.Authorization.Permissions.UpdateOrder)]
        [ProducesResponseType(typeof(CustomerDto), 200)]
        public async Task<IActionResult> AddOrderItemAsync(Guid id, CreateOrderItemModel input) {
            using (LogContext.PushProperty("OrderResourceId", id)) {
                var dto = new OrderItemDto() {
                    Sku = input.Sku,
                    Quantity = input.Quantity
                };

                var result = await service.AddOrderItemAsync(id, dto).ConfigureAwait(false);
                return Ok(result);
            }
        }
    }
}
