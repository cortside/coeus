using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Exceptions;
using Cortside.DomainEvent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.DomainService {
    public class OrderService : IOrderService {
        private readonly DatabaseContext db;
        private readonly IDomainEventOutboxPublisher publisher;
        private readonly ILogger<OrderService> logger;
        private readonly IOrderRepository orderRepository;
        private readonly IUnitOfWork uow;

        public OrderService(IOrderRepository orderRepository, DatabaseContext db, IDomainEventOutboxPublisher publisher, ILogger<OrderService> logger) {
            this.db = db;
            this.publisher = publisher;
            this.logger = logger;
            this.orderRepository = orderRepository;
            this.uow = orderRepository.UnitOfWork;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto dto) {
            // don't use context directly
            var customer = await db.Customers.FirstOrDefaultAsync(x => x.CustomerResourceId == dto.Customer.CustomerResourceId);
            //Guard.Against();
            if (customer == null) {
                throw new BadRequestMessage("customer not found");
            }

            var entity = new Order(customer, dto.Address.Street, dto.Address.City, dto.Address.State, dto.Address.Country, dto.Address.ZipCode);
            await orderRepository.AddAsync(entity);
            var @event = new OrderStateChangedEvent() { OrderResourceId = entity.OrderResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);
            await uow.SaveChangesAsync();

            return ToOrderDto(entity);
        }

        //public Task DeleteOrderAsync(int widgetId) {
        //    throw new NotImplementedException();
        //}

        public async Task<OrderDto> GetOrderAsync(Guid id) {
            var entity = await orderRepository.GetAsync(id);
            return ToOrderDto(entity);
        }

        public async Task<List<OrderDto>> GetOrdersAsync() {
            var entities = await db.Orders.ToListAsync().ConfigureAwait(false);

            var dtos = new List<OrderDto>();
            foreach (var entity in entities) {
                dtos.Add(ToOrderDto(entity));
            }

            return dtos;
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

        private OrderDto ToOrderDto(Order entity) {
            var dto = new OrderDto() {
                OrderId = entity.OrderId,
                OrderResourceId = entity.OrderResourceId,
                Address = new AddressDto() {
                    Street = entity.Address?.Street,
                    City = entity.Address?.City,
                    State = entity.Address?.State,
                    Country = entity.Address?.Country,
                    ZipCode = entity.Address?.ZipCode
                },
                Customer = new CustomerDto() {
                    FirstName = entity.Customer?.FirstName,
                    LastName = entity.Customer?.LastName,
                    Email = entity.Customer?.Email
                },
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                CreatedSubject = new SubjectDto() {
                },
                LastModifiedSubject = new SubjectDto() {
                },
                Items = new List<OrderItemDto>()
            };
            return dto;
        }
    }
}

