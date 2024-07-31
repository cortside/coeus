using System;
using System.Linq;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.EntityFramework.Searches;

namespace Acme.ShoppingCart.Data.Searches {
    public class OrderSearch : Search, IOrderSearch {
        public Guid? CustomerResourceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IQueryable<Order> Build(IQueryable<Order> entities) {
            if (CustomerResourceId.HasValue) {
                entities = entities.Where(x => x.Customer.CustomerResourceId == CustomerResourceId);
            }

            entities = FirstNameFilter(entities);
            entities = LastNameFilter(entities);

            return entities;
        }

        private IQueryable<Order> FirstNameFilter(IQueryable<Order> entities) {
            if (!string.IsNullOrEmpty(FirstName)) {
                entities = entities.Where(x => x.Customer.FirstName.StartsWith(FirstName));
            }

            return entities;
        }

        private IQueryable<Order> LastNameFilter(IQueryable<Order> entities) {
            if (!string.IsNullOrEmpty(LastName)) {
                entities = entities.Where(x => x.Customer.LastName.StartsWith(LastName));
            }

            return entities;
        }
    }
}
