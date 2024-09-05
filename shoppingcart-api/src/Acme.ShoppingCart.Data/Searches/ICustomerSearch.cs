using System;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.EntityFramework.Searches;

namespace Acme.ShoppingCart.Data.Searches {
    public interface ICustomerSearch : ISearch, ISearchBuilder<Customer> {
        Guid? CustomerResourceId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}
