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
using Cortside.DomainEvent;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.DomainService {
    public class OrderService : IOrderService {
        private readonly IDomainEventOutboxPublisher publisher;
        private readonly ILogger<OrderService> logger;
        private readonly IOrderRepository orderRepository;
        private readonly IUnitOfWork uow;
        private readonly OrderMapper mapper;
        private readonly ICustomerRepository customerRespository;

        public OrderService(IUnitOfWork uow, IOrderRepository orderRepository, ICustomerRepository customerRespository, OrderMapper mapper, IDomainEventOutboxPublisher publisher, ILogger<OrderService> logger) {
            this.publisher = publisher;
            this.logger = logger;
            this.orderRepository = orderRepository;
            this.mapper = mapper;
            this.customerRespository = customerRespository;
            this.uow = uow;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto dto) {
            // don't use context directly
            var customer = await customerRespository.GetAsync(dto.Customer.CustomerResourceId).ConfigureAwait(false);
            //Guard.Against();  // TODO: this should be in common.messages
            if (customer == null) {
                throw new BadRequestMessage("customer not found");
            }

            var entity = new Order(customer, dto.Address.Street, dto.Address.City, dto.Address.State, dto.Address.Country, dto.Address.ZipCode);
            await orderRepository.AddAsync(entity);
            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);
            await uow.SaveChangesAsync();

            return mapper.MapToDto(entity);
        }

        public async Task<OrderDto> GetOrderAsync(Guid id) {
            var entity = await orderRepository.GetAsync(id);
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

                return results;
            }
        }

        //public async Task<OrderDto> UpdateOrderAsync(OrderDto dto) {
        //    var entity = await db.Orders.FirstOrDefaultAsync(w => w.OrderId == dto.OrderId).ConfigureAwait(false);
        //    entity.FirstName = dto.FirstName;
        //    entity.LastName = dto.LastName;
        //    entity.Email = dto.Email;

        //    var @event = new OrderStageChangedEvent() { OrderId = entity.OrderId, FirstName = entity.FirstName, LastName = entity.LastName, Email = entity.Email, Timestamp = DateTime.UtcNow };
        //    await publisher.PublishAsync(@event).ConfigureAwait(false);

        //    await db.SaveChangesAsync().ConfigureAwait(false);
        //    return ToWidgetDto(entity);
        //}

        //public async Task PublishOrderStateChangedEventAsync(int id) {
        //    var entity = await db.Orders.FirstOrDefaultAsync(w => w.OrderId == id).ConfigureAwait(false);

        //    var @event = new OrderStageChangedEvent() { OrderId = entity.OrderId, FirstName = entity.FirstName, LastName = entity.LastName, Email = entity.Email, Timestamp = DateTime.UtcNow };
        //    await publisher.PublishAsync(@event).ConfigureAwait(false);
        //    await db.SaveChangesAsync().ConfigureAwait(false);
        //}
    }
}
