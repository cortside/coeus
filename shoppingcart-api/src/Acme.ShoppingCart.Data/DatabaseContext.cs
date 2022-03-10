using System.Data;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.Common.Security;
using Cortside.DomainEvent.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Acme.ShoppingCart.Data {
    public class DatabaseContext : AuditableDatabaseContext, IUnitOfWork {
        public DatabaseContext(DbContextOptions<DatabaseContext> options, ISubjectPrincipal subjectPrincipal) : base(options, subjectPrincipal) {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel) {
            return Database.BeginTransactionAsync(isolationLevel);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.AddDomainEventOutbox();

            SetDateTime(modelBuilder);
            SetCascadeDelete(modelBuilder);
        }
    }
}
