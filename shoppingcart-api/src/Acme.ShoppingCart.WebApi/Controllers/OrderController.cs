using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Facade;
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
    /// Represents the shared functionality/resources of the order resource
    /// </summary>
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Produces("application/json")]
    [ApiController]
    [Route("api/v{version:apiVersion}/orders")]
    public class OrderController : Controller {
        private readonly ILogger logger;
        private readonly OrderModelMapper orderMapper;
        private readonly IOrderFacade facade;

        /// <summary>
        /// Initializes a new instance of the OrderController
        /// </summary>
        public OrderController(ILogger<OrderController> logger, IOrderFacade facade, OrderModelMapper orderMapper) {
            this.logger = logger;
            this.facade = facade;
            this.orderMapper = orderMapper;
        }

        /// <summary>
        /// Gets orders
        /// </summary>
        [HttpGet("")]
        [Authorize(Constants.Authorization.Permissions.GetOrders)]
        [ProducesResponseType(typeof(PagedList<OrderModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrdersAsync([FromQuery] OrderSearch search, int pageNumber = 1, int pageSize = 30, string sort = null) {
            var results = await facade.SearchOrdersAsync(pageSize, pageNumber, sort ?? "", search).ConfigureAwait(false);
            return Ok(results.Convert(x => orderMapper.Map(x)));
        }

        /// <summary>
        /// Gets a order by id
        /// </summary>
        /// <param name="id">the id of the order to get</param>
        [HttpGet("{id}")]
        [ActionName(nameof(GetOrderAsync))]
        [Authorize(Constants.Authorization.Permissions.GetOrder)]
        [ProducesResponseType(typeof(OrderModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrderAsync(Guid id) {
            var dto = await facade.GetOrderAsync(id).ConfigureAwait(false);
            return Ok(orderMapper.Map(dto));
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="input"></param>
        [HttpPost("")]
        [Authorize(Constants.Authorization.Permissions.CreateOrder)]
        [ProducesResponseType(typeof(OrderModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderModel input) {
            // mapper
            var dto = new OrderDto() {
                Customer = new CustomerDto() {
                    FirstName = input.Customer.FirstName,
                    LastName = input.Customer.LastName,
                    Email = input.Customer.Email
                },
                Address = new AddressDto() {
                    Street = input.Address.Street,
                    City = input.Address.City,
                    State = input.Address.State,
                    Country = input.Address.Country,
                    ZipCode = input.Address.ZipCode
                },
                Items = input.Items?.ConvertAll(x => new OrderItemDto() { Sku = x.Sku, Quantity = x.Quantity })
            };

            var order = await facade.CreateOrderAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetOrderAsync), new { id = order.OrderResourceId }, orderMapper.Map(order));
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="input"></param>
        /// <param name="resourceId"></param>
        [HttpPost("/api/v{version:apiVersion}/customers/{resourceId}/orders")]
        [Authorize(Constants.Authorization.Permissions.CreateOrder)]
        [ProducesResponseType(typeof(OrderModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateCustomerOrderModel input, Guid resourceId) {
            var dto = new OrderDto() {
                Customer = new CustomerDto() {
                    CustomerResourceId = resourceId,
                },
                Address = new AddressDto() {
                    Street = input.Address.Street,
                    City = input.Address.City,
                    State = input.Address.State,
                    Country = input.Address.Country,
                    ZipCode = input.Address.ZipCode
                },
                Items = input.Items?.ConvertAll(x => new OrderItemDto() { Sku = x.Sku, Quantity = x.Quantity })
            };
            // convertall
            foreach (var item in input.Items ?? new List<CreateOrderItemModel>()) {
                dto.Items.Add(new OrderItemDto() { Sku = item.Sku, Quantity = item.Quantity });
            }

            var order = await facade.CreateOrderAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetOrderAsync), new { id = order.OrderResourceId }, orderMapper.Map(order));
        }

        /// <summary>
        /// Update a order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        [HttpPut("{id}")]
        [Authorize(Constants.Authorization.Permissions.UpdateOrder)]
        [ProducesResponseType(typeof(OrderModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOrderAsync(Guid id, CreateOrderModel input) {
            using (LogContext.PushProperty("OrderResourceId", id)) {
                var dto = new OrderDto() {
                    OrderResourceId = id,
                    Address = new AddressDto() {
                        Street = input.Address.Street,
                        City = input.Address.City,
                        Country = input.Address.Country,
                        State = input.Address.State,
                        ZipCode = input.Address.ZipCode
                    }
                };

                var result = await facade.UpdateOrderAsync(dto).ConfigureAwait(false);
                return Ok(orderMapper.Map(result));
            }
        }

        /// <summary>
        /// Update a order
        /// </summary>
        /// <param name="resourceId"></param>
        [HttpPost("{resourceId}/publish")]
        [Authorize(Constants.Authorization.Permissions.UpdateCustomer)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PublishCustomerStateChangedEventAsync(Guid resourceId) {
            using (LogContext.PushProperty("OrderResourceId", resourceId)) {
                await facade.PublishOrderStateChangedEventAsync(resourceId).ConfigureAwait(false);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// Add an order item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        [HttpPut("{id}/items")]
        [Authorize(Constants.Authorization.Permissions.UpdateOrder)]
        [ProducesResponseType(typeof(OrderModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddOrderItemAsync(Guid id, CreateOrderItemModel input) {
            using (LogContext.PushProperty("OrderResourceId", id)) {
                var dto = new OrderItemDto() {
                    Sku = input.Sku,
                    Quantity = input.Quantity
                };

                var result = await facade.AddOrderItemAsync(id, dto).ConfigureAwait(false);
                return Ok(result);
            }
        }
    }
}
