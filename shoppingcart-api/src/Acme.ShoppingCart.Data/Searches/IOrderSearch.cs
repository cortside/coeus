using System;
using System.Linq;
using Acme.ShoppingCart.Domain.Entities;

namespace Acme.ShoppingCart.Data.Repositories {
    public interface IOrderSearch {
        Guid? CustomerResourceId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }

        IQueryable<Order> Build(IQueryable<Order> orders);
    }
}
