using System;
using System.Linq;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.EntityFramework.Searches;

namespace Acme.ShoppingCart.Data.Searches {
    public class CustomerSearch : Search, ICustomerSearch {
        public Guid? CustomerResourceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IQueryable<Customer> Build(IQueryable<Customer> entities) {
            if (CustomerResourceId.HasValue) {
                entities = entities.Where(x => x.CustomerResourceId == CustomerResourceId);
            }

            entities = FirstNameFilter(entities);
            entities = LastNameFilter(entities);

            return entities;
        }

        private IQueryable<Customer> FirstNameFilter(IQueryable<Customer> entities) {
            if (!string.IsNullOrEmpty(FirstName)) {
                entities = entities.Where(x => x.FirstName.StartsWith(FirstName));
            }

            return entities;
        }

        private IQueryable<Customer> LastNameFilter(IQueryable<Customer> entities) {
            if (!string.IsNullOrEmpty(LastName)) {
                entities = entities.Where(x => x.LastName.StartsWith(LastName));
            }

            return entities;
        }
    }
}
