using System.Data;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Facade.Mappers;

namespace Acme.ShoppingCart.Facade {
    public class CustomerFacade : ICustomerFacade {
        private readonly IUnitOfWork uow;
        private readonly ICustomerService customerService;
        private readonly CustomerMapper mapper;

        public CustomerFacade(IUnitOfWork uow, ICustomerService customerService, CustomerMapper mapper) {
            this.uow = uow;
            this.customerService = customerService;
            this.mapper = mapper;
        }

        public async Task<CustomerDto?> CreateCustomerAsync(CustomerDto dto) {
            var customer = await customerService.CreateCustomerAsync(dto).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(customer);
        }

        public async Task<CustomerDto?> GetCustomerAsync(Guid customerResourceId) {
            var customer = await customerService.GetCustomerAsync(customerResourceId);
            return mapper.MapToDto(customer);
        }

        public async Task PublishCustomerStateChangedEventAsync(Guid resourceId) {
            await customerService.PublishCustomerStateChangedEventAsync(resourceId).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<PagedList<CustomerDto>> SearchCustomersAsync(int pageSize, int pageNumber, string sortParams, CustomerSearch search) {
            using (var tx = await uow.BeginTransactionAsync(IsolationLevel.ReadUncommitted).ConfigureAwait(false)) {
                var customers = await customerService.SearchCustomersAsync(pageSize, pageNumber, sortParams, search).ConfigureAwait(false);

                return new PagedList<CustomerDto> {
                    PageNumber = customers.PageNumber,
                    PageSize = customers.PageSize,
                    TotalItems = customers.TotalItems,
                    Items = customers.Items.ConvertAll(x => mapper.MapToDto(x))
                };
            }
        }

        public async Task<CustomerDto?> UpdateCustomerAsync(CustomerDto dto) {
            var customer = await customerService.UpdateCustomerAsync(dto).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(customer);
        }
    }
}
