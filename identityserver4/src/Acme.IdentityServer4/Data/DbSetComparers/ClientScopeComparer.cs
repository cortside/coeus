using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnerBank.IdentityServer.WebApi.Data;

namespace EnerBank.IdentityServer.WebApi.Data {
    public class ClientScopeComparer : IEqualityComparer<ClientScope> {
        public bool Equals(ClientScope x, ClientScope y) {
            return x.Scope == y.Scope;
        }

        public int GetHashCode(ClientScope obj) {
            return obj.Scope.GetHashCode() + 17;
        }
    }
}
