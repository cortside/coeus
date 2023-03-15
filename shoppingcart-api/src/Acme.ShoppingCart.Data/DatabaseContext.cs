using System.Collections.Generic;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Auditable;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.AspNetCore.EntityFramework;
using Cortside.Common.Security;
using Cortside.DomainEvent.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Acme.ShoppingCart.Data {
    public class DatabaseContext : UnitOfWorkContext<Subject>, IDatabaseContext {
        public DatabaseContext(DbContextOptions options, ISubjectPrincipal subjectPrincipal, ISubjectFactory<Subject> subjectFactory) : base(options, subjectPrincipal, subjectFactory) {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.AddDomainEventOutbox();

            base.OnModelCreating(modelBuilder);

            string descriptions = JsonConvert.SerializeObject(new[]
            {
                new KeyValuePair<string, string>("Status", "The status of the order")
            });

            modelBuilder.Entity<Order>().HasAnnotation("Descriptions", descriptions);

            SetDateTime(modelBuilder);
            SetCascadeDelete(modelBuilder);
        }
    }
}
