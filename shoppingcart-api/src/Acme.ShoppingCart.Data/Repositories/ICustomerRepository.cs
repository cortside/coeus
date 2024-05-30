using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Common.Paging;

namespace Acme.ShoppingCart.Data.Repositories {
    public interface ICustomerRepository {
        Task<Customer> AddAsync(Customer customer);
        Task<Customer> GetAsync(Guid id);
        Task<PagedList<Customer>> SearchAsync(CustomerSearch model);
    }
}
