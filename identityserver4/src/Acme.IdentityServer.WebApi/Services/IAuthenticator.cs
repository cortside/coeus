using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Models;

namespace Acme.IdentityServer.WebApi.Services {
    public interface IAuthenticator {
        Task<AuthenticationResponse> AuthenticateAsync(LoginInfo info);
    }
}
