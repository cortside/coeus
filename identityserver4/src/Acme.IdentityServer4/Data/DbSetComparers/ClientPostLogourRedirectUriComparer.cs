using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cortside.IdentityServer.WebApi.Data;

namespace Cortside.IdentityServer.WebApi.Data {
    public class ClientPostLogoutRedirectUriComparer : IEqualityComparer<ClientPostLogoutRedirectUri> {
        public bool Equals(ClientPostLogoutRedirectUri x, ClientPostLogoutRedirectUri y) {
            return x.PostLogoutRedirectUri == y.PostLogoutRedirectUri;
        }

        public int GetHashCode(ClientPostLogoutRedirectUri obj) {
            return obj.PostLogoutRedirectUri.GetHashCode() + 17;
        }
    }
}
