using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Facade.Mappers;
using Cortside.AspNetCore.Common.Paging;
using Cortside.AspNetCore.EntityFramework;

namespace Acme.ShoppingCart.Facade {
    public class OrderFacade : IOrderFacade {
        private readonly IUnitOfWork uow;
        private readonly ICustomerService customerService;
        private readonly IOrderService orderService;
        private readonly OrderMapper mapper;

        public OrderFacade(IUnitOfWork uow, ICustomerService customerService, IOrderService orderService, OrderMapper mapper) {
            this.uow = uow;
            this.customerService = customerService;
            this.orderService = orderService;
            this.mapper = mapper;
        }

        public async Task<OrderDto> AddOrderItemAsync(Guid id, OrderItemDto dto) {
            var order = await orderService.AddOrderItemAsync(id, dto).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(order);
        }

        public async Task<OrderDto> SendNotificationAsync(Guid id) {
            var order = await orderService.SendNotificationAsync(id).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(order);
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

                var results = new PagedList<OrderDto> {
                    PageNumber = orders.PageNumber,
                    PageSize = orders.PageSize,
                    TotalItems = orders.TotalItems,
                    Items = orders.Items.ConvertAll(x => mapper.MapToDto(x))
                };

                return results;
            }
        }

        public async Task<OrderDto> UpdateOrderAsync(OrderDto dto) {
            var order = await orderService.UpdateOrderAsync(dto).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(order);
        }
    }
}
