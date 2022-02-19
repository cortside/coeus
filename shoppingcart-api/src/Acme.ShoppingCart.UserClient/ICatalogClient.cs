using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.UserClient.Models.Responses;

namespace Acme.ShoppingCart.UserClient {
    public interface ICatalogClient {
        Task<CatalogItemResponse> GetUserByIdAsync(Guid userId);
    }
}
