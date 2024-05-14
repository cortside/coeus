using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;
using Cortside.AspNetCore.Common.Paging;

namespace Acme.ShoppingCart.DomainService {
    public interface IOrderService {
        Task<Order> CreateOrderAsync(Customer customer, CreateOrderDto dto);
        Task<Order> GetOrderAsync(Guid id);
        Task<PagedList<Order>> SearchOrdersAsync(OrderSearch search);
        Task PublishOrderStateChangedEventAsync(Guid id);
        Task<Order> UpdateOrderAsync(Guid id, UpdateOrderDto dto);
        Task<Order> AddOrderItemAsync(Guid id, OrderItemDto dto);
        Task<Order> SendNotificationAsync(Guid id);
    }
}
