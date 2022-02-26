using System.Threading.Tasks;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.WebApi.Facades {
    public interface IOrderFacade {
        Task<OrderDto> CreateOrderAsync(OrderDto input);
    }
}
