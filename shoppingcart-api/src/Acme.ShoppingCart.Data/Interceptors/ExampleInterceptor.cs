using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cortside.AspNetCore.Auditable;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.AspNetCore.Common;
using Cortside.AspNetCore.EntityFramework.Interceptors;
using Cortside.Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Acme.ShoppingCart.Data.Interceptors {
    /// <summary>
    /// Example to add additional logic before entities are actually saved.  Shown here for example, only implement if there is actual need.
    /// </summary>
    public class ExampleInterceptor<TSubject> : AuditableSaveChangesInterceptor<TSubject> where TSubject : Subject {

        public ExampleInterceptor(DbSet<TSubject> subjects, InternalDateTimeHandling dateTimeHandling, ISubjectPrincipal subjectPrincipal, ISubjectFactory<TSubject> subjectFactory) : base(subjects, dateTimeHandling, subjectPrincipal, subjectFactory) {
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
            // check for subject in subjects set and either create or get to attach to AudibleEntity
            var updatingSubject = await GetCurrentSubjectAsync();

            DbContext dbContext = eventData.Context;
            if (dbContext is not null) {
                var entries = dbContext.ChangeTracker.Entries().Count();
                var s = $"Change tracker has {entries} entries by user {updatingSubject.UserPrincipalName}";
                await Console.Out.WriteLineAsync(new StringBuilder(s), cancellationToken);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
