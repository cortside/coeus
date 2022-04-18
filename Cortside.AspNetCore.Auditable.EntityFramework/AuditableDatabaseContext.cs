using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Acme.ShoppingCart.Data {
    public class AuditableDatabaseContext : DbContext {
        private readonly ISubjectPrincipal subjectPrincipal;

        public AuditableDatabaseContext(DbContextOptions options, ISubjectPrincipal subjectPrincipal) : base(options) {
            this.subjectPrincipal = subjectPrincipal;
        }

        public DbSet<Subject> Subjects { get; set; }

        public Task<int> SaveChangesAsync() {
            SetAuditableEntityValues();
            return base.SaveChangesAsync(default);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            SetAuditableEntityValues();
            return base.SaveChangesAsync(true, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) {
            SetAuditableEntityValues();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Use SaveChangesAsync
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override int SaveChanges() {
            throw new NotImplementedException("intentially not implemented, use Async methods");
        }

        private void SetAuditableEntityValues() {
            // check for subject in subjects set and either create or get to attach to AudibleEntity
            var updatingSubject = GetCurrentSubject();
            ChangeTracker.DetectChanges();
            var modified = ChangeTracker.Entries().Where(x => x.Entity is AuditableEntity && (x.State == EntityState.Modified || x.State == EntityState.Added));
            var added = ChangeTracker.Entries().Where(x => x.Entity is AuditableEntity && x.State == EntityState.Added);

#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
            foreach (var item in modified) {
                ((AuditableEntity)item.Entity).LastModifiedSubject = updatingSubject;
                ((AuditableEntity)item.Entity).LastModifiedDate = DateTime.Now.ToUniversalTime();
            }

            foreach (var item in added) {
                ((AuditableEntity)(item.Entity)).CreatedSubject = updatingSubject;
                ((AuditableEntity)(item.Entity)).CreatedDate = DateTime.Now.ToUniversalTime();
            }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
        }

        /// <summary>
        /// Gets or creates the current subject record.
        /// </summary>
        /// <returns></returns>
        private Subject GetCurrentSubject() {
            var subjectId = Guid.Parse(subjectPrincipal.SubjectId);

            var subject = Subjects.Local.FirstOrDefault(s => s.SubjectId == subjectId);
            subject ??= Subjects.FirstOrDefault(s => s.SubjectId == subjectId);

            // create new subject if one is not found
            if (subject == null) {
                subject = new Subject(subjectId, subjectPrincipal.GivenName, subjectPrincipal.FamilyName, subjectPrincipal.Name, subjectPrincipal.UserPrincipalName);
                Subjects.Add(subject);
            }
            return subject;
        }

        protected static void SetDateTime(ModelBuilder builder) {
            // 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM
            var min = new DateTime(1753, 1, 1, 0, 0, 0);
            var max = new DateTime(9999, 12, 31, 23, 59, 59);

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
#pragma warning disable S3358 // Ternary operators should not be nested
                v => v < min ? min : v > max ? max : v,
                v => v < min ? min : v > max ? max : v);
#pragma warning restore S3358 // Ternary operators should not be nested

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
#pragma warning disable S3358 // Ternary operators should not be nested
                v => v.HasValue ? v < min ? min : v > max ? max : v : v,
                v => v.HasValue ? v < min ? min : v > max ? max : v : v);
#pragma warning restore S3358 // Ternary operators should not be nested

            foreach (var entityType in builder.Model.GetEntityTypes()) {
                foreach (var property in entityType.GetProperties()) {
                    if (property.ClrType == typeof(DateTime)) {
                        property.SetValueConverter(dateTimeConverter);
                    } else if (property.ClrType == typeof(DateTime?)) {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }
        }

        public static void SetCascadeDelete(ModelBuilder builder) {
            var fks = builder.Model.GetEntityTypes().SelectMany(t => t.GetDeclaredForeignKeys());
            foreach (var fk in fks) {
                fk.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }
    }
}
