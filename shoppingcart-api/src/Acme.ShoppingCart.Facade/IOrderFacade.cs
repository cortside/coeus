using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Dto;
using Cortside.AspNetCore.Common.Paging;

namespace Acme.ShoppingCart.Facade {
    public interface IOrderFacade {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto input);
        Task<OrderDto> GetOrderAsync(Guid id);
        Task<PagedList<OrderDto>> SearchOrdersAsync(OrderSearchDto search);
        Task PublishOrderStateChangedEventAsync(Guid id);
        Task<OrderDto> UpdateOrderAsync(Guid id, UpdateOrderDto dto);
        Task<OrderDto> AddOrderItemAsync(Guid id, OrderItemDto dto);
        Task<OrderDto> SendNotificationAsync(Guid id);
    }
}
