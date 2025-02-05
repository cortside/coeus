using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto.Input;
using Acme.ShoppingCart.Dto.Output;
using Acme.ShoppingCart.Dto.Search;
using Acme.ShoppingCart.Facade.Mappers;
using Cortside.AspNetCore.Common.Paging;
using Cortside.AspNetCore.EntityFramework;
using Medallion.Threading;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.Facade {
    public class CustomerFacade : ICustomerFacade {
        private readonly IUnitOfWork uow;
        private readonly ICustomerService customerService;
        private readonly CustomerMapper mapper;
        private readonly ILogger<CustomerFacade> logger;
        private readonly IDistributedLockProvider lockProvider;

        public CustomerFacade(ILogger<CustomerFacade> logger, IUnitOfWork uow, ICustomerService customerService, CustomerMapper mapper, IDistributedLockProvider lockProvider) {
            this.uow = uow;
            this.customerService = customerService;
            this.mapper = mapper;
            this.logger = logger;
            this.lockProvider = lockProvider;
        }

        private static string GetLockName(Guid id) {
            return $"CustomerResourceId:{id}";
        }

        public async Task<CustomerDto> CreateCustomerAsync(UpdateCustomerDto dto) {
            var customer = await customerService.CreateCustomerAsync(dto).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            return mapper.MapToDto(customer);
        }

        public async Task<CustomerDto> GetCustomerAsync(Guid resourceId) {
            // Using BeginNoTracking on GET endpoints for a single entity so that data is read committed
            // with assumption that it might be used for changes in future calls
            await using (var tx = uow.BeginNoTracking()) {
                var customer = await customerService.GetCustomerAsync(resourceId);
                return mapper.MapToDto(customer);
            }
        }

        public async Task PublishCustomerStateChangedEventAsync(Guid resourceId) {
            await customerService.PublishCustomerStateChangedEventAsync(resourceId).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<PagedList<CustomerDto>> SearchCustomersAsync(CustomerSearchDto search) {
            var customerSearch = mapper.Map(search);
            // Using BeginReadUncommittedAsync on GET endpoints that return a list, this will read uncommitted and
            // AsNoTracking in ef core.  This will result in a non-blocking dirty read, which is accepted best practice for mssql.
            await using (var tx = await uow.BeginReadUncommitedAsync().ConfigureAwait(false)) {
                var customers = await customerService.SearchCustomersAsync(customerSearch).ConfigureAwait(false);

                var result = customers.Convert(x => mapper.MapToDto(x));
                return result;
            }
        }

        public async Task<CustomerDto> UpdateCustomerAsync(Guid resourceId, UpdateCustomerDto dto) {
            var lockName = GetLockName(resourceId);

            logger.LogDebug("Acquiring lock for {LockName}", lockName);
            await using (await lockProvider.AcquireLockAsync(lockName).ConfigureAwait(false)) {
                logger.LogDebug("Acquired lock for {LockName}", lockName);

                var customer = await customerService.UpdateCustomerAsync(resourceId, dto).ConfigureAwait(false);
                await uow.SaveChangesAsync().ConfigureAwait(false);

                return mapper.MapToDto(customer);
            }
        }
    }
}
