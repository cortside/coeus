using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;

namespace Acme.ShoppingCart.Data.Repositories {
    public interface IOrderRepository : IRepository<Order> {
        Task<Order> AddAsync(Order order);

        Task<Order> UpdateAsync(Order order);

        Task<Order> GetAsync(Guid id);
    }
}
