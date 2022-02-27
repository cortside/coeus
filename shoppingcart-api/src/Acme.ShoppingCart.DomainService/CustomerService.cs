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
using Cortside.DomainEvent;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.DomainService {
    public class CustomerService : ICustomerService {
        private readonly IUnitOfWork uow;
        private readonly CustomerMapper mapper;
        private readonly IDomainEventOutboxPublisher publisher;
        private readonly ILogger<CustomerService> logger;
        private readonly ICustomerRepository customerRepository;

        public CustomerService(IUnitOfWork uow, ICustomerRepository customerRepository, CustomerMapper mapper, IDomainEventOutboxPublisher publisher, ILogger<CustomerService> logger) {
            this.uow = uow;
            this.mapper = mapper;
            this.publisher = publisher;
            this.logger = logger;
            this.customerRepository = customerRepository;
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto dto) {
            var entity = new Customer(dto.FirstName, dto.LastName, dto.Email);
            await customerRepository.AddAsync(entity).ConfigureAwait(false);

            var @event = new CustomerStateChangedEvent() { CustomerResourceId = entity.CustomerResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(entity);
        }

        public async Task<CustomerDto> GetCustomerAsync(Guid customerResourceId) {
            var entity = await customerRepository.GetAsync(customerResourceId).ConfigureAwait(false);
            return mapper.MapToDto(entity);
        }

        public async Task<PagedList<CustomerDto>> SearchCustomersAsync(int pageSize, int pageNumber, string sortParams, CustomerSearch search) {
            using (var tx = await uow.BeginTransactionAsync(IsolationLevel.ReadUncommitted).ConfigureAwait(false)) {
                var customers = await customerRepository.SearchAsync(pageSize, pageNumber, sortParams, search).ConfigureAwait(false);

                var results = new PagedList<CustomerDto> {
                    PageNumber = customers.PageNumber,
                    PageSize = customers.PageSize,
                    TotalItems = customers.TotalItems,
                    Items = customers.Items.Select(x => mapper.MapToDto(x)).ToList()
                };

                await tx.CommitAsync().ConfigureAwait(false);
                return results;
            }
        }

        public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto dto) {
            var entity = await customerRepository.GetAsync(dto.CustomerResourceId).ConfigureAwait(false);
            entity.Update(dto.FirstName, dto.LastName, dto.Email);

            var @event = new CustomerStateChangedEvent() { CustomerResourceId = entity.CustomerResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            await uow.SaveChangesAsync().ConfigureAwait(false);
            return mapper.MapToDto(entity);
        }

        public async Task PublishCustomerStateChangedEventAsync(Guid resourceId) {
            var entity = await customerRepository.GetAsync(resourceId).ConfigureAwait(false);

            var @event = new CustomerStateChangedEvent() { CustomerResourceId = entity.CustomerResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
