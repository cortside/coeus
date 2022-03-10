using System;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;
using Cortside.DomainEvent;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.DomainService {
    public class CustomerService : ICustomerService {
        private readonly IDomainEventOutboxPublisher publisher;
        private readonly ILogger<CustomerService> logger;
        private readonly ICustomerRepository customerRepository;

        public CustomerService(ICustomerRepository customerRepository, IDomainEventOutboxPublisher publisher, ILogger<CustomerService> logger) {
            this.publisher = publisher;
            this.logger = logger;
            this.customerRepository = customerRepository;
        }

        public async Task<Customer> CreateCustomerAsync(CustomerDto dto) {
            var entity = new Customer(dto.FirstName, dto.LastName, dto.Email);
            customerRepository.Add(entity);

            var @event = new CustomerStateChangedEvent() { CustomerResourceId = entity.CustomerResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            return entity;
        }

        public async Task<Customer> GetCustomerAsync(Guid customerResourceId) {
            var entity = await customerRepository.GetAsync(customerResourceId).ConfigureAwait(false);
            return entity;
        }

        public Task<PagedList<Customer>> SearchCustomersAsync(int pageSize, int pageNumber, string sortParams, CustomerSearch search) {
            return customerRepository.SearchAsync(pageSize, pageNumber, sortParams, search);
        }

        public async Task<Customer> UpdateCustomerAsync(CustomerDto dto) {
            var entity = await customerRepository.GetAsync(dto.CustomerResourceId).ConfigureAwait(false);
            entity.Update(dto.FirstName, dto.LastName, dto.Email);

            var @event = new CustomerStateChangedEvent() { CustomerResourceId = entity.CustomerResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            return entity;
        }

        public async Task PublishCustomerStateChangedEventAsync(Guid resourceId) {
            var entity = await customerRepository.GetAsync(resourceId).ConfigureAwait(false);

            var @event = new CustomerStateChangedEvent() { CustomerResourceId = entity.CustomerResourceId, Timestamp = entity.LastModifiedDate };
            await publisher.PublishAsync(@event).ConfigureAwait(false);
        }
    }
}
