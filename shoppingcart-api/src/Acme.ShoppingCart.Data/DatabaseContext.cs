using System.Data.Common;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.Common.Security;
using Cortside.DomainEvent.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Acme.ShoppingCart.Data {
    public class DatabaseContext : AuditableDatabaseContext, IUnitOfWork, IDatabaseContext {
        public DatabaseContext(DbContextOptions<DatabaseContext> options, ISubjectPrincipal subjectPrincipal) : base(options, subjectPrincipal) {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        public Task<IDbContextTransaction> BeginReadUncommitedAsync() {
            return Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted);
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(System.Data.IsolationLevel isolationLevel) {
            return Database.BeginTransactionAsync(isolationLevel);
        }

        public IExecutionStrategy CreateExecutionStrategy() {
            return Database.CreateExecutionStrategy();
        }

        public DbTransaction GetDbTransaction() {
            return Database.CurrentTransaction?.GetDbTransaction();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.AddDomainEventOutbox();

            SetDateTime(modelBuilder);
            SetCascadeDelete(modelBuilder);
        }
    }
}
