using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.DomainService {
    public interface ICustomerService {
        Task<CustomerDto> CreateCustomerAsync(CustomerDto dto);
        Task<CustomerDto> GetCustomerAsync(Guid customerResourceId);
        Task<PagedList<CustomerDto>> SearchCustomersAsync(int pageSize, int pageNumber, string sortParams, CustomerSearch search);
        Task<CustomerDto> UpdateCustomerAsync(CustomerDto dto);
        Task PublishCustomerStateChangedEventAsync(Guid resourceId);
    }
}
