using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.UserClient.Models.Responses;

namespace Acme.ShoppingCart.UserClient {
    public interface IUserClient {
        Task<UserInfoResponse> GetUserByIdAsync(Guid userId);
    }
}
