using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Common.Paging;

namespace Acme.ShoppingCart.Data.Repositories {
    public interface IOrderRepository : IRepository<Order> {
        Task<PagedList<Order>> SearchAsync(int pageSize, int pageNumber, string sortParams, IOrderSearch model);
        Order Add(Order order);
        Task<Order> GetAsync(Guid id);
    }
}
