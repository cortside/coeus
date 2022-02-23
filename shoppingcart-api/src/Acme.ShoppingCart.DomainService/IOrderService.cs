using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.DomainService {
    public interface IOrderService {
        Task<OrderDto> CreateOrderAsync(OrderDto dto);
        Task<OrderDto> GetOrderAsync(Guid id);
        Task<PagedList<OrderDto>> SearchOrdersAsync(int pageSize, int pageNumber, string sortParams, OrderSearch search);
        Task PublishOrderStateChangedEventAsync(Guid id);
        Task<OrderDto> UpdateOrderAsync(OrderDto dto);
        Task<OrderDto> AddOrderItemAsync(Guid id, OrderItemDto dto);
    }
}
