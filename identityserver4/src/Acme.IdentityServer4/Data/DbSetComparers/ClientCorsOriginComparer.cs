﻿using System.Collections.Generic;

namespace Cortside.IdentityServer.WebApi.Data {
    public class ClientCorsOriginComparer : IEqualityComparer<ClientCorsOrigin> {
        public bool Equals(ClientCorsOrigin x, ClientCorsOrigin y) {
            return x.Origin == y.Origin;
        }

        public int GetHashCode(ClientCorsOrigin obj) {
            return obj.Origin.GetHashCode() + 17;
        }
    }
}
