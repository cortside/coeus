using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Common.Models;

namespace Acme.ShoppingCart.DomainService
{
    public interface ICustomerTypeService
    {
        Task<ListResult<CustomerType>> GetCustomerTypesAsync();
    }
}
