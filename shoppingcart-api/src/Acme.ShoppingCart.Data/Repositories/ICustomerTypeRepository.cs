using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Common.Models;

namespace Acme.ShoppingCart.Data.Repositories {
    public interface ICustomerTypeRepository {
        Task<ListResult<CustomerType>> GetCustomerTypesAsync();
    }
}
