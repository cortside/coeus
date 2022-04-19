using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;
using Cortside.AspNetCore.Common.Paging;

namespace Acme.ShoppingCart.DomainService {
    public interface IOrderService {
        Task<Order> CreateOrderAsync(Customer customer, OrderDto dto);
        Task<Order> GetOrderAsync(Guid id);
        Task<PagedList<Order>> SearchOrdersAsync(int pageSize, int pageNumber, string sortParams, OrderSearch search);
        Task PublishOrderStateChangedEventAsync(Guid id);
        Task<Order> UpdateOrderAsync(OrderDto dto);
        Task<Order> AddOrderItemAsync(Guid id, OrderItemDto dto);
    }
}
