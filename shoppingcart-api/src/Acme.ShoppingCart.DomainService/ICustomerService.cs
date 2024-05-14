using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;
using Cortside.AspNetCore.Common.Paging;

namespace Acme.ShoppingCart.DomainService {
    public interface ICustomerService {
        Task<Customer> CreateCustomerAsync(UpdateCustomerDto dto);
        Task<Customer> GetCustomerAsync(Guid customerResourceId);
        Task<PagedList<Customer>> SearchCustomersAsync(CustomerSearch search);
        Task<Customer> UpdateCustomerAsync(Guid resourceId, UpdateCustomerDto dto);
        Task PublishCustomerStateChangedEventAsync(Guid resourceId);
    }
}
