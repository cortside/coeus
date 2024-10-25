using Acme.ShoppingCart.Data.Interceptors;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Auditable;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.AspNetCore.Common;
using Cortside.AspNetCore.EntityFramework;
using Cortside.Common.Security;
using Cortside.DomainEvent.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.Data {
    public class DatabaseContext : UnitOfWorkContext<Subject>, IDatabaseContext {
        public DatabaseContext(DbContextOptions options, ISubjectPrincipal subjectPrincipal, ISubjectFactory<Subject> subjectFactory) : base(options, subjectPrincipal, subjectFactory) {
            // example of how to set an InternalDateTimeHandling other than the default (which is Utc)
            DateTimeHandling = InternalDateTimeHandling.Utc;
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.AddDomainEventOutbox();
            modelBuilder.SetDateTime();
            modelBuilder.SetCascadeDelete();
        }

        /// <summary>
        /// Example of how to set a custom interceptor, method only needed if an interceptor is added
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.AddInterceptors(new ExampleInterceptor<Subject>(Subjects, DateTimeHandling, subjectPrincipal, subjectFactory));
            base.OnConfiguring(optionsBuilder);
        }
    }
}
