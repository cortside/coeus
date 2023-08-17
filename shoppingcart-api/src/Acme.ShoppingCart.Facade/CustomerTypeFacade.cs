using System.Linq;
using System.Threading.Tasks;
using Acme.ShoppingCart.DomainService;
using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Facade.Mappers;
using Cortside.AspNetCore.Common.Models;
using Cortside.AspNetCore.EntityFramework;

namespace Acme.ShoppingCart.Facade {
    public class CustomerTypeFacade : ICustomerTypeFacade {
        private readonly IUnitOfWork uow;
        private readonly ICustomerTypeService customerService;
        private readonly CustomerTypeMapper mapper;

        public CustomerTypeFacade(IUnitOfWork uow, ICustomerTypeService customerService, CustomerTypeMapper mapper) {
            this.uow = uow;
            this.customerService = customerService;
            this.mapper = mapper;
        }

        public async Task<ListResult<CustomerTypeDto>> GetCustomerTypesAsync() {
            // Using BeginNoTracking on GET endpoints for a single entity so that data is read committed
            // with assumption that it might be used for changes in future calls
            using (var tx = uow.BeginNoTracking()) {
                var customerTypes = await customerService.GetCustomerTypesAsync().ConfigureAwait(false);
                return new ListResult<CustomerTypeDto> {
                    Results = customerTypes.Results.Select(x => mapper.MapToDto(x)).ToList()
                };
            }
        }
    }
}
