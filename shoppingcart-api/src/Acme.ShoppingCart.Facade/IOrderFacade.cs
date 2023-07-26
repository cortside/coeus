using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.Dto;
using Cortside.AspNetCore.Common.Paging;

namespace Acme.ShoppingCart.Facade {
    public interface IOrderFacade {
        Task<OrderDto> CreateOrderAsync(OrderDto input);
        Task<OrderDto> GetOrderAsync(Guid id);
        Task<PagedList<OrderDto>> SearchOrdersAsync(int pageSize, int pageNumber, string sortParams, OrderSearch search);
        Task PublishOrderStateChangedEventAsync(Guid id);
        Task<OrderDto> UpdateOrderAsync(OrderDto dto);
        Task<OrderDto> AddOrderItemAsync(Guid id, OrderItemDto dto);
        Task<OrderDto> SendNotificationAsync(Guid id);
    }
}
