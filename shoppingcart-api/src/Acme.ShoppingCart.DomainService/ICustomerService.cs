using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.DomainService {
    public interface ICustomerService {
        Task<Customer> CreateCustomerAsync(CustomerDto dto);
        Task<Customer> GetCustomerAsync(Guid customerResourceId);
        Task<PagedList<Customer>> SearchCustomersAsync(int pageSize, int pageNumber, string sortParams, CustomerSearch search);
        Task<Customer> UpdateCustomerAsync(CustomerDto dto);
        Task PublishCustomerStateChangedEventAsync(Guid resourceId);
    }
}
