using System.Threading.Tasks;
using EnerBank.IdentityServer.Data;
using EnerBank.IdentityServer.WebApi.Models;

namespace EnerBank.IdentityServer.Services {
    public interface IAuthenticator {
        Task<AuthenticationResponse> AuthenticateAsync(LoginInfo info);
    }
}
