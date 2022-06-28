﻿using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cortside.IdentityServer.WebApi.Data {

    internal class DataProtectionKeyContext : DbContext, IDataProtectionKeyContext {

        // A recommended constructor overload when using EF Core
        // with dependency injection.
        public DataProtectionKeyContext(DbContextOptions<DataProtectionKeyContext> options)
            : base(options) { }

        // This maps to the table that stores keys.
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}
