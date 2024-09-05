using System;
using System.Linq;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.TestUtilities;
using Cortside.AspNetCore.Auditable.Entities;

namespace Acme.ShoppingCart.WebApi.IntegrationTests {
    public static class DatabaseFixture {
        public static void SeedInMemoryDb(DatabaseContext dbContext) {
            var subject = new Subject(Guid.Empty, string.Empty, string.Empty, string.Empty, "system");
            if (!dbContext.Subjects.Any(x => x.SubjectId == subject.SubjectId)) {
                dbContext.Subjects.Add(subject);

				var customer = EntityBuilder.GetCustomerEntity();
				dbContext.Customers.Add(customer);

                // intentionally using this override to avoid the not implemented exception
                dbContext.SaveChanges(true);
            }
        }
    }
}
