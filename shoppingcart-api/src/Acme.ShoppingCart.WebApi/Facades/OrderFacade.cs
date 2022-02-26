using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.WebApi.Facades {
    public class OrderFacade : IOrderFacade {
        private readonly IUnitOfWork uow;
        private readonly ICustomerService customerService;
        private readonly IOrderService orderService;

        public OrderFacade(IUnitOfWork uow, ICustomerService customerService, IOrderService orderService) {
            this.uow = uow;
            this.customerService = customerService;
            this.orderService = orderService;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto input) {
            if (input.Customer.CustomerResourceId == Guid.Empty) {
                input.Customer = await customerService.CreateCustomerAsync(input.Customer).ConfigureAwait(false);
            } else {
                input.Customer = await customerService.GetCustomerAsync(input.Customer.CustomerResourceId).ConfigureAwait(false);
            }

            var order = await orderService.CreateOrderAsync(input).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return order;
        }
    }
}
