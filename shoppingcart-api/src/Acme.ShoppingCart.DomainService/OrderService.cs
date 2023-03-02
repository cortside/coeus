using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.CatalogApi;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Exceptions;
using Cortside.AspNetCore.Common.Paging;
using Cortside.Common.Validation;
using Cortside.DomainEvent;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Acme.ShoppingCart.DomainService {
    public class OrderService : IOrderService {
        private readonly IDomainEventOutboxPublisher publisher;
        private readonly ILogger<OrderService> logger;
        private readonly IOrderRepository orderRepository;
        private readonly ICatalogClient catalog;

        public OrderService(IOrderRepository orderRepository, IDomainEventOutboxPublisher publisher, ILogger<OrderService> logger, ICatalogClient catalog) {
            this.publisher = publisher;
            this.logger = logger;
            this.orderRepository = orderRepository;
            this.catalog = catalog;
        }

        public async Task<Order> CreateOrderAsync(Customer customer, OrderDto dto) {
            Guard.From.Null<BadRequestMessage>(customer, "customer not found");

            var entity = new Order(customer, dto.Address.Street, dto.Address.City, dto.Address.State, dto.Address.Country, dto.Address.ZipCode);
            using (LogContext.PushProperty("OrderResourceId", entity.OrderResourceId)) {
                foreach (var i in dto.Items) {
                    var item = await catalog.GetItemAsync(i.Sku).ConfigureAwait(false);
                    entity.AddItem(item, i.Quantity);
                }
                logger.LogInformation("Created new order");

                orderRepository.Add(entity);
                var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = entity.LastModifiedDate };
                await publisher.PublishAsync(@event).ConfigureAwait(false);

                return entity;
            }
        }

        public async Task<Order> GetOrderAsync(Guid id) {
            var entity = await orderRepository.GetAsync(id).ConfigureAwait(false);
            return entity;
        }

        public Task<PagedList<Order>> SearchOrdersAsync(int pageSize, int pageNumber, string sortParams, OrderSearch search) {
            return orderRepository.SearchAsync(pageSize, pageNumber, sortParams, search);
        }

        public async Task<Order> UpdateOrderAsync(OrderDto dto) {
            var entity = await orderRepository.GetAsync(dto.OrderResourceId).ConfigureAwait(false);
            entity.UpdateAddress(dto.Address.Street, dto.Address.City, dto.Address.State, dto.Address.Country, dto.Address.ZipCode);

            // remove items not in dto
            var itemsToRemove = new List<OrderItem>();
            foreach (var item in entity.Items.Where(x => !dto.Items.Any(i => i.Sku == x.Sku))) {
                itemsToRemove.Add(item);
            }
            entity.RemoveItems(itemsToRemove);

            // add items not already on order
            foreach (var item in dto.Items.Where(x => !entity.Items.Any(i => i.Sku == x.Sku))) {
                var catalogItem = await catalog.GetItemAsync(item.Sku).ConfigureAwait(false);
                entity.AddItem(catalogItem, item.Quantity);
            }

            // update any existing items with quantity changed
            foreach (var item in dto.Items.Where(x => entity.Items.Any(i => i.Sku == x.Sku))) {
                var i = entity.Items.First(i => i.Sku != item.Sku);
                if (i.Quantity != item.Quantity) {
                    entity.UpdateItem(i, item.Quantity);
                }
            }

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
            var item = await catalog.GetItemAsync(dto.Sku).ConfigureAwait(false);
            entity.AddItem(item, dto.Quantity);
            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = DateTime.UtcNow };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            return entity;
        }
    }
}
