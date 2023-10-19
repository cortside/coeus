using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Cortside.AspNetCore.Auditable;
using Cortside.Common.Security;
using Acme.IdentityServer.WebApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Acme.IdentityServer.WebApi {
    /// <summary>
    /// Design time context factory for EF
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityServerDbContext> {
        /// <summary>
        /// Create context
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public IdentityServerDbContext CreateDbContext(string[] args) {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetSection("Database").GetValue<string>("ConnectionString");

            var builder = new DbContextOptionsBuilder<IdentityServerDbContext>();
            builder.UseSqlServer(connectionString);

            Guid subjectId = Guid.NewGuid();
            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipal(subjectId, new List<Claim>());
            SubjectPrincipal principal = new SubjectPrincipal(claimsPrincipal);
            return new IdentityServerDbContext(builder.Options, principal, new HttpContextAccessor(), new DefaultSubjectFactory());
        }

        private ClaimsPrincipal GetClaimsPrincipal(Guid subjectId, List<Claim> claims) {
            List<Claim> allClaims = new List<Claim>(claims);
            allClaims.Add(new Claim("sub", subjectId.ToString()));
            var identity = new ClaimsIdentity(allClaims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return claimsPrincipal;
        }
    }
}
