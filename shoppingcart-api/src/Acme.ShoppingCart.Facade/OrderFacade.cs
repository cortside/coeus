using System.Data;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Facade.Mappers;

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

        public async Task<OrderDto?> AddOrderItemAsync(Guid id, OrderItemDto dto) {
            var order = await orderService.AddOrderItemAsync(id, dto).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(order);
        }

        public async Task<OrderDto?> CreateOrderAsync(OrderDto input) {
            Customer customer;
            if (input.Customer.CustomerResourceId == Guid.Empty) {
                customer = await customerService.CreateCustomerAsync(input.Customer).ConfigureAwait(false);
            } else {
                customer = await customerService.GetCustomerAsync(input.Customer.CustomerResourceId).ConfigureAwait(false);
            }

            var order = await orderService.CreateOrderAsync(customer, input).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(order);
        }

        public async Task<OrderDto?> GetOrderAsync(Guid id) {
            var order = await orderService.GetOrderAsync(id).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(order);
        }

        public async Task PublishOrderStateChangedEventAsync(Guid id) {
            await orderService.PublishOrderStateChangedEventAsync(id).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<PagedList<OrderDto>> SearchOrdersAsync(int pageSize, int pageNumber, string sortParams, OrderSearch search) {
            using (var tx = await uow.BeginTransactionAsync(IsolationLevel.ReadUncommitted).ConfigureAwait(false)) {
                var orders = await orderService.SearchOrdersAsync(pageSize, pageNumber, sortParams, search).ConfigureAwait(false);

                var results = new PagedList<OrderDto> {
                    PageNumber = orders.PageNumber,
                    PageSize = orders.PageSize,
                    TotalItems = orders.TotalItems,
                    Items = orders.Items.ConvertAll(x => mapper.MapToDto(x))
                };

                return results;
            }
        }

        public async Task<OrderDto?> UpdateOrderAsync(OrderDto dto) {
            var order = await orderService.UpdateOrderAsync(dto).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(order);
        }
    }
}
