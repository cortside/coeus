using System.Collections.Generic;
using Acme.ShoppingCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Acme.ShoppingCart.Data {
    public interface IDatabaseContext {
        DbSet<Customer> Customers { get; set; }
        DbSet<Order> Orders { get; set; }

        void RemoveRange(IEnumerable<object> entities);
        EntityEntry Remove(object entity);
    }
}
