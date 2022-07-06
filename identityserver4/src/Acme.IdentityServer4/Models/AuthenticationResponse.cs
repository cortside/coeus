using Cortside.IdentityServer.Data;

namespace Cortside.IdentityServer.WebApi.Models {
    public class AuthenticationResponse {
        public User User { get; set; }
        public string Error { get; set; }
    }
}
