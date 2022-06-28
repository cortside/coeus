using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cortside.IdentityServer.WebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace Cortside.IdentityServer.Data {
    /// <summary>
    /// Interface for User DB Context.
    /// </summary>
    public interface IIdentityServerDbContext : IDisposable {
        void AddUser(User user);
        void UpdateUser(User user);
        void AddRole(Role role);

        Task SaveChangesAsync();
        void SaveChanges();

        void RemoveRange(IEnumerable<object> entities);

        IQueryable<User> Users { get; }
        IQueryable<Role> Roles { get; }


        DbSet<ApiScope> ApiScopes { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<ClientClaim> ClientClaims { get; set; }
        DbSet<ClientScope> ClientScopes { get; set; }
        DbSet<ClientSecret> ClientSecrets { get; set; }
        DbSet<ClientSecretRequest> ClientSecretRequests { get; set; }
        DbSet<ClientGrantType> ClientGrantTypes { get; set; }
    }
}
