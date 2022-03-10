using System;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Exceptions;
using Acme.ShoppingCart.UserClient;
using Cortside.DomainEvent;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.DomainService {
    public class OrderService : IOrderService {
        private readonly IDomainEventOutboxPublisher publisher;
        private readonly ILogger<OrderService> logger;
        private readonly IOrderRepository orderRepository;
        private readonly ICatalogClient catalog;
        private readonly ICustomerRepository customerRespository;

        public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRespository, IDomainEventOutboxPublisher publisher, ILogger<OrderService> logger, ICatalogClient catalog) {
            this.publisher = publisher;
            this.logger = logger;
            this.orderRepository = orderRepository;
            this.customerRespository = customerRespository;
            this.catalog = catalog;
        }

        public async Task<Order> CreateOrderAsync(Customer customer, OrderDto dto) {
            //Guard.Against();  // TODO: this should be in common.messages
            if (customer == null) {
                throw new BadRequestMessage("customer not found");
            }

            var entity = new Order(customer, dto.Address.Street, dto.Address.City, dto.Address.State, dto.Address.Country, dto.Address.ZipCode);
            foreach (var i in dto.Items) {
                var item = await catalog.GetItem(i.Sku).ConfigureAwait(false);
                entity.AddItem(item, i.Quantity);
            }
            await orderRepository.AddAsync(entity).ConfigureAwait(false);
            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            return entity;
        }

        public async Task<Order> GetOrderAsync(Guid id) {
            var entity = await orderRepository.GetAsync(id).ConfigureAwait(false);
            return entity;
        }

        public async Task<PagedList<Order>> SearchOrdersAsync(int pageSize, int pageNumber, string sortParams, OrderSearch search) {
            return await orderRepository.SearchAsync(pageSize, pageNumber, sortParams, search).ConfigureAwait(false);
        }

        public async Task<Order> UpdateOrderAsync(OrderDto dto) {
            var entity = await orderRepository.GetAsync(dto.OrderResourceId).ConfigureAwait(false);
            // TODO: add method to update order

            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = DateTime.UtcNow };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            return entity;
        }

        public async Task PublishOrderStateChangedEventAsync(Guid id) {
            var entity = await orderRepository.GetAsync(id).ConfigureAwait(false);

            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = DateTime.UtcNow };
            await publisher.PublishAsync(@event).ConfigureAwait(false);
        }

        public async Task<Order> AddOrderItemAsync(Guid id, OrderItemDto dto) {
            var entity = await orderRepository.GetAsync(id).ConfigureAwait(false);
            var item = await catalog.GetItem(dto.Sku).ConfigureAwait(false);
            entity.AddItem(item, dto.Quantity);
            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = DateTime.UtcNow };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            return entity;
        }
    }
}
