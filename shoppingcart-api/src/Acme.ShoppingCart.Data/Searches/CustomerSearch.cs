using System;
using System.Linq;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Domain.Entities;

namespace Acme.ShoppingCart.Data.Repositories {
    public class CustomerSearch : ISearchBuilder<Customer>, ICustomerSearch {
        public Guid? CustomerResourceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IQueryable<Customer> Build(IQueryable<Customer> customers) {
            if (CustomerResourceId.HasValue) {
                customers = customers.Where(x => x.CustomerResourceId == CustomerResourceId);
            }

            customers = FirstNameFilter(customers);
            customers = LastNameFilter(customers);

            return customers;
        }

        private IQueryable<Customer> FirstNameFilter(IQueryable<Customer> customers) {
            if (!string.IsNullOrEmpty(FirstName)) {
                customers = customers.Where(x => x.FirstName.StartsWith(FirstName));
            }

            return customers;
        }

        private IQueryable<Customer> LastNameFilter(IQueryable<Customer> customers) {
            if (!string.IsNullOrEmpty(LastName)) {
                customers = customers.Where(x => x.LastName.StartsWith(LastName));
            }

            return customers;
        }
    }
}
