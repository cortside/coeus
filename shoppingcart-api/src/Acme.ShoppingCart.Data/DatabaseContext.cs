using System.Threading;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.Common.Security;
using Cortside.DomainEvent.EntityFramework;
using Microsoft.EntityFrameworkCore;
namespace Acme.ShoppingCart.Data {
    public class DatabaseContext : AuditableDatabaseContext, IUnitOfWork {

        public DatabaseContext(DbContextOptions<DatabaseContext> options, ISubjectPrincipal subjectPrincipal) : base(options, subjectPrincipal) {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default) {
            throw new System.NotImplementedException();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.AddDomainEventOutbox();

            //modelBuilder.Entity<Order>(x => {
            //    x.HasMany(y => y.Items);
            //    x.HasOne(y => y.Customer);
            //    x.HasOne(y => y.Address);

            //    x.HasOne(y => y.CreatedSubject);
            //    x.HasOne(y => y.LastModifiedSubject);
            //});

            SetDateTime(modelBuilder);
            SetCascadeDelete(modelBuilder);
        }
    }
}
