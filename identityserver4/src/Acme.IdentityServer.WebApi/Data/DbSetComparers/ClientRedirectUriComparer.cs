using System.Collections.Generic;

namespace Acme.IdentityServer.WebApi.Data.DbSetComparers {
    public class ClientRedirectUriComparer : IEqualityComparer<ClientRedirectUri> {
        public bool Equals(ClientRedirectUri x, ClientRedirectUri y) {
            return x.RedirectUri == y.RedirectUri;
        }

        public int GetHashCode(ClientRedirectUri obj) {
            return obj.RedirectUri.GetHashCode() + 17;
        }
    }
}
