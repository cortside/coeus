using Acme.ShoppingCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.Data {
    public interface IDatabaseContext {
        DbSet<CustomerType> CustomerTypes { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<Order> Orders { get; set; }
    }
}
