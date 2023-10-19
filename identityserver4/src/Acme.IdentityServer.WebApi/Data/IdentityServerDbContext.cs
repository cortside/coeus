using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Cortside.AspNetCore.Auditable;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.AspNetCore.EntityFramework;
using Cortside.Common.Security;
using Cortside.DomainEvent.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Acme.IdentityServer.WebApi.Data {
    public class IdentityServerDbContext : DbContext, IIdentityServerDbContext, IUnitOfWork {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISubjectPrincipal subjectPrincipal;

        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options, ISubjectPrincipal subjectPrincipal, IHttpContextAccessor httpContextAccessor, ISubjectFactory<Subject> subjectFactory) : base(options) {
            this.subjectPrincipal = subjectPrincipal;
            _httpContextAccessor = httpContextAccessor;
        }

        //TODO: Move these registrations into their own mapping classes as in prior versions of EF.
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.AddDomainEventOutbox("dbo");

            modelBuilder.HasDefaultSchema("auth");
            modelBuilder.Entity<User>(x => { x.ToTable("User"); });
            modelBuilder.Entity<Role>(x => { x.ToTable("Role"); });
            modelBuilder.Entity<UserRole>(x => {
                x.ToTable("UserRole").HasKey(ur => new { ur.UserId, ur.RoleId });
                x.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);
                x.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            });
            modelBuilder.Entity<UserClaim>(x => {
                x.ToTable("UserClaim").HasKey(uc => uc.UserClaimId);
                x.HasOne(ur => ur.User)
                    .WithMany(u => u.UserClaims)
                    .HasForeignKey(ur => ur.UserId);
            });
            modelBuilder.Entity<LoginAttempt>(x => {
                x.ToTable("LoginAttempts").HasKey(la => la.Id);
                x.HasOne(ur => ur.User)
                    .WithMany(u => u.LoginAttempts)
                    .HasForeignKey(la => la.UserId);
            });

            // register all tables to the DB context
            modelBuilder.Entity<Client>(x => { x.ToTable("Clients"); });
            modelBuilder.Entity<ClientGrantType>(x => { x.ToTable("ClientGrantTypes"); });
            modelBuilder.Entity<ClientPostLogoutRedirectUri>(x => { x.ToTable("ClientPostLogoutRedirectUris"); });
            modelBuilder.Entity<ClientRedirectUri>(x => { x.ToTable("ClientRedirectUris"); });
            modelBuilder.Entity<ClientCorsOrigin>(x => { x.ToTable("ClientCorsOrigins"); });
            modelBuilder.Entity<ClientScope>(x => { x.ToTable("ClientScopes"); });
            modelBuilder.Entity<ClientClaim>(x => { x.ToTable("ClientClaims"); });
            modelBuilder.Entity<ClientSecretRequest>(x => { x.ToTable("ClientSecretRequest"); });
            modelBuilder.Entity<ClientSecret>(x => {
                x.ToTable("ClientSecrets").HasKey("Id");
                x.HasOne(cs => cs.Client).WithOne(c => c.ClientSecret);
            });
            modelBuilder.Entity<ClientGrantType>(x => {
                x.ToTable("ClientGrantTypes").HasKey("Id");
            });
        }

        public DbSet<ApiScope> ApiScopes { set; get; }
        public DbSet<User> Users { set; get; }
        public DbSet<Role> Roles { set; get; }
        public DbSet<UserRole> UserRoles { set; get; }
        public DbSet<UserClaim> UserClaims { set; get; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientClaim> ClientClaims { get; set; }
        public DbSet<ClientScope> ClientScopes { get; set; }
        public DbSet<ClientSecret> ClientSecrets { get; set; }
        public DbSet<ClientSecretRequest> ClientSecretRequests { get; set; }
        public DbSet<ClientGrantType> ClientGrantTypes { get; set; }
        IQueryable<User> IIdentityServerDbContext.Users { get { return Users; } }
        IQueryable<Role> IIdentityServerDbContext.Roles { get { return Roles; } }
        IQueryable<LoginAttempt> IIdentityServerDbContext.LoginAttempts { get { return LoginAttempts; } }

        public void AddUser(User user) {
            Users.Add(user);
        }
        public void UpdateUser(User user) {
            Users.Update(user);
        }

        public void AddRole(Role role) {
            Roles.Add(role);
        }

        public Task SaveChangesAsync() {
            SetAuditableEntityValues();
            return base.SaveChangesAsync(true);
        }

        public static Guid GuidParse(string str) {
            if (string.IsNullOrEmpty(str)) {
                return Guid.Empty;
            }
            Guid guid = Guid.Parse(str); // Will throw if not a valid Guid
            return guid;
        }

        public new void SaveChanges() {
            SetAuditableEntityValues();
            base.SaveChanges(true);
        }

        private void SetAuditableEntityValues() {
            DateTime timestamp = DateTime.UtcNow;
            var currentUser = GetCurrentUser();
            Guid subjectId = currentUser != null ? GuidParse(currentUser) : GuidParse(subjectPrincipal.SubjectId);

            foreach (var entry in ChangeTracker.Entries().Where(e =>
                         e.State == EntityState.Added || e.State == EntityState.Deleted || e.State == EntityState.Modified)) {
                var auditable = entry.Entity as AuditableEntity;
                if (auditable == null) {
                    continue;
                }

                if (entry.State == EntityState.Added) {
                    auditable.CreateDate = timestamp;
                    auditable.CreateUserId = subjectId;
                }

                auditable.LastModifiedDate = timestamp;
                auditable.LastModifiedUserId = subjectId;
            }
        }

        private string GetCurrentUser() {
            // IHttpContextAccessor will have be be setup in IoC and injected into context constructor
            if (_httpContextAccessor != null) {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null) {
                    // If it returns null, even when the user was authenticated, you may try to get the value of a specific claim
                    var authenticatedUserId = httpContext.User.FindFirst("act")?.Value ?? httpContext.User.FindFirst("sub")?.Value;
                    if (authenticatedUserId != null) {
                        return authenticatedUserId;
                    }
                }
            }
            //If not authenticated return null.
            return null;
        }

        public new void RemoveRange(IEnumerable<object> entities) {
            base.RemoveRange(entities);
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel) {
            throw new NotImplementedException();
        }

        public Task<IDbContextTransaction> BeginReadUncommitedAsync() {
            throw new NotImplementedException();
        }

        public IDbContextTransaction BeginNoTracking() {
            throw new NotImplementedException();
        }

        public IExecutionStrategy CreateExecutionStrategy() {
            throw new NotImplementedException();
        }
    }
}
