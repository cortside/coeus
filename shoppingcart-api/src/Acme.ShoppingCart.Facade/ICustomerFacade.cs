using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.Facade {
    public interface ICustomerFacade {
        Task<CustomerDto?> CreateCustomerAsync(CustomerDto dto);
        Task<CustomerDto?> GetCustomerAsync(Guid customerResourceId);
        Task<PagedList<CustomerDto>> SearchCustomersAsync(int pageSize, int pageNumber, string sortParams, CustomerSearch search);
        Task<CustomerDto?> UpdateCustomerAsync(CustomerDto dto);
        Task PublishCustomerStateChangedEventAsync(Guid resourceId);
    }
}
