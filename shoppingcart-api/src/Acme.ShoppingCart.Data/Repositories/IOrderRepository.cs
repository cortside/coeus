using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Domain.Entities;

namespace Acme.ShoppingCart.Data.Repositories {
    public interface IOrderRepository : IRepository<Order> {
        Task<PagedList<Order>> SearchAsync(int pageSize, int pageNumber, string sortParams, IOrderSearch model);
        Task<Order> AddAsync(Order order);
        Task<Order> GetAsync(Guid id);
    }
}
