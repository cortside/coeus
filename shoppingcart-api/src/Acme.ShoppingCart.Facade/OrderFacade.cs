using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto.Input;
using Acme.ShoppingCart.Dto.Output;
using Acme.ShoppingCart.Dto.Search;
using Acme.ShoppingCart.Facade.Mappers;
using Cortside.AspNetCore.Common.Paging;
using Cortside.AspNetCore.EntityFramework;
using Medallion.Threading;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.Facade {
    public class OrderFacade : IOrderFacade {
        private readonly IUnitOfWork uow;
        private readonly ICustomerService customerService;
        private readonly IOrderService orderService;
        private readonly OrderMapper mapper;
        private readonly IDistributedLockProvider lockProvider;
        private readonly ILogger<OrderFacade> logger;

        public OrderFacade(ILogger<OrderFacade> logger, IUnitOfWork uow, ICustomerService customerService, IOrderService orderService, OrderMapper mapper, IDistributedLockProvider lockProvider) {
            this.logger = logger;
            this.uow = uow;
            this.customerService = customerService;
            this.orderService = orderService;
            this.mapper = mapper;
            this.lockProvider = lockProvider;
        }

        private static string GetLockName(Guid id) {
            return $"OrderResourceId:{id}";
        }

        public async Task<OrderDto> AddOrderItemAsync(Guid id, OrderItemDto dto) {
            var order = await orderService.AddOrderItemAsync(id, dto).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(order);
        }

        public async Task<OrderDto> SendNotificationAsync(Guid id) {
            var lockName = GetLockName(id);

            logger.LogDebug("Acquiring lock for {LockName}", lockName);
            await using (await lockProvider.AcquireLockAsync(lockName).ConfigureAwait(false)) {
                logger.LogDebug("Acquired lock for {LockName}", lockName);

                var order = await orderService.SendNotificationAsync(id).ConfigureAwait(false);
                await uow.SaveChangesAsync().ConfigureAwait(false);

                return mapper.MapToDto(order);
            }
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto input) {
            Customer customer;
            if (input.Customer.CustomerResourceId == Guid.Empty) {
                var createCustomerDto = new UpdateCustomerDto() {
                    FirstName = input.Customer.FirstName,
                    LastName = input.Customer.LastName,
                    Email = input.Customer.Email
                };
                customer = await customerService.CreateCustomerAsync(createCustomerDto).ConfigureAwait(false);
            } else {
                customer = await customerService.GetCustomerAsync(input.Customer.CustomerResourceId).ConfigureAwait(false);
            }

            var order = await orderService.CreateOrderAsync(customer, input).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(order);
        }

        public async Task<OrderDto> GetOrderAsync(Guid id) {
            // Using BeginNoTracking on GET endpoints for a single entity so that data is read committed
            // with assumption that it might be used for changes in future calls
            await using (var tx = uow.BeginNoTracking()) {
                var order = await orderService.GetOrderAsync(id).ConfigureAwait(false);

                return mapper.MapToDto(order);
            }
        }

        public async Task PublishOrderStateChangedEventAsync(Guid id) {
            await orderService.PublishOrderStateChangedEventAsync(id).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<PagedList<OrderDto>> SearchOrdersAsync(OrderSearchDto search) {
            // Using BeginReadUncommittedAsync on GET endpoints that return a list, this will read uncommitted and
            // as notracking in ef core.  this will result in a non-blocking dirty read, which is accepted best practice for mssql
            await using (var tx = await uow.BeginReadUncommitedAsync().ConfigureAwait(false)) {
                var orders = await orderService.SearchOrdersAsync(mapper.Map(search)).ConfigureAwait(false);

                var results = orders.Convert(x => mapper.MapToDto(x));
                return results;
            }
        }

        public async Task<OrderDto> UpdateOrderAsync(Guid id, UpdateOrderDto dto) {
            var lockName = GetLockName(id);

            logger.LogDebug("Acquiring lock for {LockName}", lockName);
            await using (await lockProvider.AcquireLockAsync(lockName).ConfigureAwait(false)) {
                logger.LogDebug("Acquired lock for {LockName}", lockName);

                var order = await orderService.UpdateOrderAsync(id, dto).ConfigureAwait(false);
                await uow.SaveChangesAsync().ConfigureAwait(false);

                return mapper.MapToDto(order);
            }
        }

        public async Task CancelOrderAsync(Guid id) {
            var lockName = GetLockName(id);

            logger.LogDebug("Acquiring lock for {LockName}", lockName);
            await using (await lockProvider.AcquireLockAsync(lockName).ConfigureAwait(false)) {
                logger.LogDebug("Acquired lock for {LockName}", lockName);
                await orderService.CancelOrderAsync(id).ConfigureAwait(false);
                await uow.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
