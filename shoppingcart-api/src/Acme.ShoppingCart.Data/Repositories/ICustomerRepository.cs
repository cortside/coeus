using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Domain.Entities;

namespace Acme.ShoppingCart.Data.Repositories {
    public interface ICustomerRepository : IRepository<Customer> {
        Customer Add(Customer customer);
        Task<Customer> GetAsync(Guid id);
        Task<PagedList<Customer>> SearchAsync(int pageSize, int pageNumber, string sortParams, CustomerSearch model);
    }
}
