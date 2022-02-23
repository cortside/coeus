using System.IO;
using Acme.ShoppingCart.Data;
using Cortside.Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Acme.ShoppingCart.WebApi {
    /// <summary>
    /// Design time context factory for EF
    /// https://codingblast.com/entityframework-core-idesigntimedbcontextfactory/
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext> {
        private readonly ISubjectPrincipal subjectPrincipal;

        /// <summary>
        /// DesignTimeDbContextFactory
        /// </summary>
        public DesignTimeDbContextFactory() {
        }

        /// <summary>
        /// DesignTimeDbContextFactory constructor
        /// </summary>
        /// <param name="subjectPrincipal"></param>
        public DesignTimeDbContextFactory(ISubjectPrincipal subjectPrincipal) {
            this.subjectPrincipal = subjectPrincipal;
        }

        /// <summary>
        /// Create context
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DatabaseContext CreateDbContext(string[] args) {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetSection("Database").GetValue<string>("ConnectionString");

            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseSqlServer(connectionString);

            return new DatabaseContext(builder.Options, subjectPrincipal);
        }
    }
}
