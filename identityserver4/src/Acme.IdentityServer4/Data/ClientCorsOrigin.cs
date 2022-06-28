using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cortside.IdentityServer.WebApi.Data
{
    public class ClientCorsOrigin {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string Origin { get; set; } = ""; // default value should not be null
    }
}
