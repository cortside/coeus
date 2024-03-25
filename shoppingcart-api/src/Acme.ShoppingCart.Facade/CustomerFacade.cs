using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Facade.Mappers;
using Cortside.AspNetCore.Common.Paging;
using Cortside.AspNetCore.EntityFramework;

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

        public async Task<CustomerDto> CreateCustomerAsync(CustomerDto dto) {
            var customer = await customerService.CreateCustomerAsync(dto).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(customer);
        }

        public async Task<CustomerDto> GetCustomerAsync(Guid customerResourceId) {
            // Using BeginNoTracking on GET endpoints for a single entity so that data is read committed
            // with assumption that it might be used for changes in future calls
            await using (var tx = uow.BeginNoTracking()) {
                var customer = await customerService.GetCustomerAsync(customerResourceId);
                return mapper.MapToDto(customer);
            }
        }

        public async Task PublishCustomerStateChangedEventAsync(Guid resourceId) {
            await customerService.PublishCustomerStateChangedEventAsync(resourceId).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<PagedList<CustomerDto>> SearchCustomersAsync(int pageSize, int pageNumber, string sortParams, CustomerSearch search) {
            // Using BeginReadUncommittedAsync on GET endpoints that return a list, this will read uncommitted and
            // as notracking in ef core.  this will result in a non-blocking dirty read, which is accepted best practice for mssql
            await using (var tx = await uow.BeginReadUncommitedAsync().ConfigureAwait(false)) {
                var customers = await customerService.SearchCustomersAsync(pageSize, pageNumber, sortParams, search).ConfigureAwait(false);

                return new PagedList<CustomerDto> {
                    PageNumber = customers.PageNumber,
                    PageSize = customers.PageSize,
                    TotalItems = customers.TotalItems,
                    Items = customers.Items.ConvertAll(x => mapper.MapToDto(x))
                };
            }
        }

        public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto dto) {
            var customer = await customerService.UpdateCustomerAsync(dto).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(customer);
        }
    }
}
