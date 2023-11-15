using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Data;

namespace Acme.IdentityServer.WebApi.Models {
    public class AuthenticationResponse {
        public User User { get; set; }
        public string Error { get; set; }
    }
}
