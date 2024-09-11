using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Common.Paging;

namespace Acme.ShoppingCart.Data.Repositories {
    public interface IOrderRepository {
        Task<PagedList<Order>> SearchAsync(IOrderSearch model);
        Task<Order> AddAsync(Order order);
        Task<Order> GetAsync(Guid id);
        void RemoveItems(List<OrderItem> itemsToRemove);
    }
}
