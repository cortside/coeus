using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.Facade {
    public interface IOrderFacade {
        Task<OrderDto?> CreateOrderAsync(OrderDto input);
        Task<OrderDto?> GetOrderAsync(Guid id);
        Task<PagedList<OrderDto>> SearchOrdersAsync(int pageSize, int pageNumber, string sortParams, OrderSearch search);
        Task PublishOrderStateChangedEventAsync(Guid id);
        Task<OrderDto?> UpdateOrderAsync(OrderDto dto);
        Task<OrderDto?> AddOrderItemAsync(Guid id, OrderItemDto dto);
    }
}
