using System.Threading.Tasks;
using Acme.ShoppingCart.Dto;
using Cortside.AspNetCore.Common.Models;

namespace Acme.ShoppingCart.Facade
{
    public interface ICustomerTypeFacade
    {
        Task<ListResult<CustomerTypeDto>> GetCustomerTypesAsync();
    }
}
