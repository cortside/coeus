using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Acme.ShoppingCart.Data {
    public interface IUnitOfWork : IDisposable {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);

        //Task<IDbContextTransaction> BeginScopeAsync();
        //bool IsInScope();
        //Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
