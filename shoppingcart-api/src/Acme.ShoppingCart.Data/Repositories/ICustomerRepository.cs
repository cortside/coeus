using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Domain.Entities;

namespace Acme.ShoppingCart.Data.Repositories {
    public interface ICustomerRepository {
        IUnitOfWork UnitOfWork { get; }

        Task<Order> AddAsync(Order order);
        Task<Order> GetAsync(Guid id);
        Task<PagedList<Customer>> SearchAsync(int pageSize, int pageNumber, string sortParams, CustomerSearch model);
    }
}