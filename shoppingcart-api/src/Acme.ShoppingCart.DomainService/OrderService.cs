using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.DomainService.Mappers;
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
        private readonly IUnitOfWork uow;
        private readonly ICatalogClient catalog;
        private readonly OrderMapper mapper;
        private readonly ICustomerRepository customerRespository;

        public OrderService(IUnitOfWork uow, IOrderRepository orderRepository, ICustomerRepository customerRespository, OrderMapper mapper, IDomainEventOutboxPublisher publisher, ILogger<OrderService> logger, ICatalogClient catalog) {
            this.publisher = publisher;
            this.logger = logger;
            this.orderRepository = orderRepository;
            this.mapper = mapper;
            this.customerRespository = customerRespository;
            this.uow = uow;
            this.catalog = catalog;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto dto) {
            // don't use context directly
            var customer = await customerRespository.GetAsync(dto.Customer.CustomerResourceId).ConfigureAwait(false);
            //Guard.Against();  // TODO: this should be in common.messages
            if (customer == null) {
                throw new BadRequestMessage("customer not found");
            }

            var entity = new Order(customer, dto.Address.Street, dto.Address.City, dto.Address.State, dto.Address.Country, dto.Address.ZipCode);
            foreach (var i in dto.Items) {
                var item = await catalog.GetItem(i.Sku).ConfigureAwait(false);
                entity.AddItem(item, i.Quantity);
            }
            await orderRepository.AddAsync(entity);
            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);
            await uow.SaveChangesAsync();

            return mapper.MapToDto(entity);
        }

        public async Task<OrderDto> GetOrderAsync(Guid id) {
            var entity = await orderRepository.GetAsync(id).ConfigureAwait(false);
            return mapper.MapToDto(entity);
        }

        public async Task<PagedList<OrderDto>> SearchOrdersAsync(int pageSize, int pageNumber, string sortParams, OrderSearch search) {
            using (var tx = await uow.BeginTransactionAsync(IsolationLevel.ReadUncommitted)) {
                var orders = await orderRepository.SearchAsync(pageSize, pageNumber, sortParams, search).ConfigureAwait(false);

                var results = new PagedList<OrderDto> {
                    PageNumber = orders.PageNumber,
                    PageSize = orders.PageSize,
                    TotalItems = orders.TotalItems,
                    Items = orders.Items.Select(x => mapper.MapToDto(x)).ToList()
                };

                await tx.CommitAsync().ConfigureAwait(false);
                return results;
            }
        }

        public async Task<OrderDto> UpdateOrderAsync(OrderDto dto) {
            var entity = await orderRepository.GetAsync(dto.OrderResourceId).ConfigureAwait(false);
            //entity.FirstName = dto.FirstName;
            //entity.LastName = dto.LastName;
            //entity.Email = dto.Email;

            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = DateTime.UtcNow };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            await uow.SaveChangesAsync().ConfigureAwait(false);
            return mapper.MapToDto(entity);
        }

        public async Task PublishOrderStateChangedEventAsync(Guid id) {
            var entity = await orderRepository.GetAsync(id).ConfigureAwait(false);

            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = DateTime.UtcNow };
            await publisher.PublishAsync(@event).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<OrderDto> AddOrderItemAsync(Guid id, OrderItemDto dto) {
            var entity = await orderRepository.GetAsync(id).ConfigureAwait(false);
            var item = await catalog.GetItem(dto.Sku).ConfigureAwait(false);
            entity.AddItem(item, dto.Quantity);
            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = DateTime.UtcNow };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            await uow.SaveChangesAsync().ConfigureAwait(false);
            return mapper.MapToDto(entity);
        }
    }
}
