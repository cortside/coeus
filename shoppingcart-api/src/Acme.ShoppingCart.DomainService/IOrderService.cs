using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.DomainService {
    public interface IOrderService {
        Task<OrderDto> CreateOrderAsync(OrderDto dto);
        //Task DeleteOrderAsync(int widgetId);
        Task<OrderDto> GetOrderAsync(Guid id);
        Task<PagedList<OrderDto>> SearchOrdersAsync(int pageSize, int pageNumber, string sortParams, OrderSearch search);
        //Task PublishOrderStateChangedEventAsync(int id);
        //Task<OrderDto> UpdateOrderAsync(OrderDto dto);
    }
}
