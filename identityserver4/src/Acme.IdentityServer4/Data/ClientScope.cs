﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cortside.IdentityServer.WebApi.Data
{
    public class ClientScope {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string Scope { get; set; } = ""; // default value should not be null
    }
}
