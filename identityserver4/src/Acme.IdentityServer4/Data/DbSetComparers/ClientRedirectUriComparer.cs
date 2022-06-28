using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cortside.IdentityServer.WebApi.Data;

namespace Cortside.IdentityServer.WebApi.Data {
    public class ClientRedirectUriComparer : IEqualityComparer<ClientRedirectUri> {
        public bool Equals(ClientRedirectUri x, ClientRedirectUri y) {
            return x.RedirectUri == y.RedirectUri;
        }

        public int GetHashCode(ClientRedirectUri obj) {
            return obj.RedirectUri.GetHashCode() + 17;
        }
    }
}
