using System;
using Acme.ShoppingCart.Domain.Entities;

namespace Acme.ShoppingCart.Data.Searches {
    public interface IOrderSearch : ISearch, ISearchBuilder<Order> {
        Guid? CustomerResourceId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}
