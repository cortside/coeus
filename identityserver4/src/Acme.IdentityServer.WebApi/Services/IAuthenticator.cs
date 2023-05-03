using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Models;

namespace Acme.IdentityServer.Services {
    public interface IAuthenticator {
        Task<AuthenticationResponse> AuthenticateAsync(LoginInfo info);
    }
}
