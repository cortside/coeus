using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Acme.ShoppingCart.Data;
using Cortside.AspNetCore.Auditable;
using Cortside.Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Acme.ShoppingCart.WebApi {
    /// <summary>
    /// Design time context factory for EF
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext> {
        /// <summary>
        /// Create context
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DatabaseContext CreateDbContext(string[] args) {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetSection("Database").GetValue<string>("ConnectionString");

            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseSqlServer(connectionString);

            var principal = new SubjectPrincipal(new List<Claim>() { new Claim("sub", Guid.Empty.ToString()) });
            return new DatabaseContext(builder.Options, principal, new DefaultSubjectFactory());
        }
    }
}
