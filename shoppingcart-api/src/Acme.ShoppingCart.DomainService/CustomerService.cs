using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;
using Cortside.DomainEvent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.DomainService {
    public class CustomerService : ICustomerService {
        private readonly DatabaseContext db;
        private readonly IDomainEventOutboxPublisher publisher;
        private readonly ILogger<CustomerService> logger;

        public CustomerService(DatabaseContext db, IDomainEventOutboxPublisher publisher, ILogger<CustomerService> logger) {
            this.db = db;
            this.publisher = publisher;
            this.logger = logger;
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto dto) {
            var entity = new Customer() {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };

            // user initiated transaction with retry strategy set needs to execute in new strategy 
            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {
                // need a transaction and 2 savechanges so that I have the id for the widget in the event
                using (var tx = await db.Database.BeginTransactionAsync().ConfigureAwait(false)) {
                    try {
                        db.Customers.Add(entity);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                        var @event = new CustomerStateChangedEvent() { CustomerResourceId = entity.CustomerResourceId, Timestamp = entity.LastModifiedDate };
                        await publisher.PublishAsync(@event).ConfigureAwait(false);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                        await tx.CommitAsync().ConfigureAwait(false);
                    } catch (Exception ex) {
                        logger.LogError(ex, "unhandled exception");
                        await tx.RollbackAsync().ConfigureAwait(false);
                        throw;
                    }
                }
            }).ConfigureAwait(false);

            return ToCustomerDto(entity);
        }

        public Task<CustomerDto> DeleteCustomerAsync(int widgetId) {
            throw new NotImplementedException();
        }

        public async Task<CustomerDto> GetCustomerAsync(Guid customerResourceId) {
            var entity = await db.Customers.SingleAsync(x => x.CustomerResourceId == customerResourceId).ConfigureAwait(false);
            return ToCustomerDto(entity);
        }

        public async Task<List<CustomerDto>> GetCustomersAsync() {
            var entities = await db.Customers.ToListAsync().ConfigureAwait(false);

            var dtos = new List<CustomerDto>();
            foreach (var entity in entities) {
                dtos.Add(ToCustomerDto(entity));
            }

            return dtos;
        }

        public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto dto) {
            var entity = await db.Customers.FirstOrDefaultAsync(w => w.CustomerId == dto.CustomerId).ConfigureAwait(false);
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;

            var @event = new CustomerStateChangedEvent() { CustomerResourceId = entity.CustomerResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            await db.SaveChangesAsync().ConfigureAwait(false);
            return ToCustomerDto(entity);
        }

        public async Task PublishCustomerStateChangedEventAsync(int id) {
            var entity = await db.Customers.FirstOrDefaultAsync(w => w.CustomerId == id).ConfigureAwait(false);

            var @event = new CustomerStateChangedEvent() { CustomerResourceId = entity.CustomerResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }

        private CustomerDto ToCustomerDto(Customer entity) {
            return new CustomerDto() {
                CustomerId = entity.CustomerId,
                CustomerResourceId = entity.CustomerResourceId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email
            };
        }
    }
}
