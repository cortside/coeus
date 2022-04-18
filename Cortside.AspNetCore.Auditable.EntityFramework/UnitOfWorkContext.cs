using System.Threading.Tasks;
using Cortside.Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Acme.ShoppingCart.Data {
    public class UnitOfWorkContext : AuditableDatabaseContext, IUnitOfWork {
        public UnitOfWorkContext(DbContextOptions options, ISubjectPrincipal subjectPrincipal) : base(options, subjectPrincipal) {
        }

        public Task<IDbContextTransaction> BeginReadUncommitedAsync() {
            return Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted);
        }

        public IUnitOfWork BeginNoTracking() {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return this;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(System.Data.IsolationLevel isolationLevel) {
            // default to no tracking when reading uncommitted with assumption/expectation that data will be used in read only fashion
            if (isolationLevel == System.Data.IsolationLevel.ReadUncommitted) {
                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }

            return Database.BeginTransactionAsync(isolationLevel);
        }

        public IExecutionStrategy CreateExecutionStrategy() {
            return Database.CreateExecutionStrategy();
        }
    }
}
