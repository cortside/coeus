using System.Collections.Generic;

namespace Acme.IdentityServer.WebApi.Data {
    public class ClientPostLogoutRedirectUriComparer : IEqualityComparer<ClientPostLogoutRedirectUri> {
        public bool Equals(ClientPostLogoutRedirectUri x, ClientPostLogoutRedirectUri y) {
            return x.PostLogoutRedirectUri == y.PostLogoutRedirectUri;
        }

        public int GetHashCode(ClientPostLogoutRedirectUri obj) {
            return obj.PostLogoutRedirectUri.GetHashCode() + 17;
        }
    }
}
