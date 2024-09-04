using System;
using System.Net;
using System.Threading.Tasks;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Facade;
using Acme.ShoppingCart.WebApi.Mappers;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Asp.Versioning;
using Cortside.AspNetCore.Common.Paging;
using Cortside.Common.Messages.MessageExceptions;
using Medallion.Threading;
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
    public class OrderController : ControllerBase {
        private readonly OrderModelMapper orderMapper;
        private readonly IOrderFacade facade;
        private readonly IDistributedLockProvider lockProvider;
        private readonly ILogger<OrderController> logger;

        /// <summary>
        /// Initializes a new instance of the OrderController
        /// </summary>
        public OrderController(IOrderFacade facade, OrderModelMapper orderMapper, ILogger<OrderController> logger, IDistributedLockProvider lockProvider) {
            this.facade = facade;
            this.orderMapper = orderMapper;
            this.lockProvider = lockProvider;
            this.logger = logger;
        }

        /// <summary>
        /// Gets orders
        /// </summary>
        [HttpGet("")]
        [Authorize(Constants.Authorization.Permissions.GetOrders)]
        [ProducesResponseType(typeof(PagedList<OrderModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrdersAsync([FromQuery] OrderSearchModel search) {
            var searchDto = orderMapper.MapToDto(search);
            var results = await facade.SearchOrdersAsync(searchDto).ConfigureAwait(false);
            return Ok(results.Convert(x => orderMapper.Map(x)));
        }

        /// <summary>
        /// Gets an order by id
        /// </summary>
        /// <param name="id">the id of the order to get</param>
        [HttpGet("{id:guid}")]
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
            var dto = orderMapper.MapToDto(input);
            var order = await facade.CreateOrderAsync(dto).ConfigureAwait(false);
            return CreatedAtAction(nameof(GetOrderAsync), new { id = order.OrderResourceId }, orderMapper.Map(order));
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="input"></param>
        /// <param name="resourceId"></param>
        [HttpPost("/api/v{version:apiVersion}/customers/{resourceId:guid}/orders")]
        [Authorize(Constants.Authorization.Permissions.CreateOrder)]
        [ProducesResponseType(typeof(OrderModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCustomerOrderAsync([FromBody] CreateCustomerOrderModel input, Guid resourceId) {
            using (LogContext.PushProperty("CustomerResourceId", resourceId)) {
                var dto = orderMapper.MapToDto(input);
                dto.Customer.CustomerResourceId = resourceId;

                var order = await facade.CreateOrderAsync(dto).ConfigureAwait(false);
                return CreatedAtAction(nameof(GetOrderAsync), new { id = order.OrderResourceId }, orderMapper.Map(order));
            }
        }

        /// <summary>
        /// Update an order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        [HttpPut("{id}")]
        [Authorize(Constants.Authorization.Permissions.UpdateOrder)]
        [ProducesResponseType(typeof(OrderModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOrderAsync(Guid id, UpdateOrderModel input) {
            var lockName = $"OrderResourceId:{id}";
            logger.LogDebug("Acquiring lock for {LockName}", lockName);
            await using (await lockProvider.AcquireLockAsync(lockName).ConfigureAwait(false)) {
                logger.LogDebug("Acquired lock for {LockName}", lockName);
                using (LogContext.PushProperty("OrderResourceId", id)) {
                    var dto = orderMapper.MapToDto(input);

                    OrderDto result;
                    try {
                        result = await facade.UpdateOrderAsync(id, dto).ConfigureAwait(false);
                    } catch (Exception ex) {
                        throw new InternalServerErrorResponseException("Unable to update order", ex);
                    }

                    return Ok(orderMapper.Map(result));
                }
            }
        }

        /// <summary>
        /// Update an order
        /// </summary>
        /// <param name="resourceId"></param>
        [HttpPost("{resourceId}/publish")]
        [Authorize(Constants.Authorization.Permissions.PublishOrder)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PublishOrderStateChangedEventAsync(Guid resourceId) {
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
        [HttpPost("{id}/items")]
        [Authorize(Constants.Authorization.Permissions.UpdateOrder)]
        [ProducesResponseType(typeof(OrderModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddOrderItemAsync(Guid id, CreateOrderItemModel input) {
            using (LogContext.PushProperty("OrderResourceId", id)) {
                var dto = orderMapper.MapToDto(input);
                var result = await facade.AddOrderItemAsync(id, dto).ConfigureAwait(false);
                return Ok(orderMapper.Map(result));
            }
        }
    }
}
