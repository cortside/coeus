using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Common.Models;

namespace Acme.ShoppingCart.DomainService {
    public class CustomerTypeService : ICustomerTypeService {
        private readonly ICustomerTypeRepository customerRepository;

        public CustomerTypeService(ICustomerTypeRepository customerRepository) {
            this.customerRepository = customerRepository;
        }

        public async Task<ListResult<CustomerType>> GetCustomerTypesAsync() {
            var results = await customerRepository.GetCustomerTypesAsync().ConfigureAwait(false);
            return results;
        }
    }
}
