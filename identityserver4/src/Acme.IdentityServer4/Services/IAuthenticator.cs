using System.Threading.Tasks;
using Cortside.IdentityServer.WebApi.Models;

namespace Cortside.IdentityServer.Services {
    public interface IAuthenticator {
        Task<AuthenticationResponse> AuthenticateAsync(LoginInfo info);
    }
}
