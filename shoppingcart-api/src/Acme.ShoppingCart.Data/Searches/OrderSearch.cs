using System;
using System.Linq;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.Data.Repositories {
    public class OrderSearch : ISearchBuilder<Order>, IOrderSearch {
        public Guid? CustomerResourceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IQueryable<Order> Build(IQueryable<Order> orders) {
            orders.Include(x => x.Customer)
                .Include(x => x.Address);

            if (CustomerResourceId.HasValue) {
                orders = orders.Where(x => x.Customer.CustomerResourceId == CustomerResourceId);
            }

            orders = FirstNameFilter(orders);
            orders = LastNameFilter(orders);

            return orders;
        }

        private IQueryable<Order> FirstNameFilter(IQueryable<Order> orders) {
            if (!string.IsNullOrEmpty(FirstName)) {
                orders = orders.Where(x => x.Customer.FirstName.StartsWith(FirstName));
            }

            return orders;
        }

        private IQueryable<Order> LastNameFilter(IQueryable<Order> orders) {
            if (!string.IsNullOrEmpty(LastName)) {
                orders = orders.Where(x => x.Customer.LastName.StartsWith(LastName));
            }

            return orders;
        }
    }
}
