using System;
using System.Linq;
using Acme.ShoppingCart.Domain.Entities;

namespace Acme.ShoppingCart.Data.Searches {
    public interface ICustomerSearch {
        Guid? CustomerResourceId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }

        IQueryable<Customer> Build(IQueryable<Customer> customers);
    }
}
