using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.Data.Repositories {
    public class CustomerRepository : ICustomerRepository {
        private readonly IDatabaseContext context;

        public CustomerRepository(IDatabaseContext context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PagedList<Customer>> SearchAsync(int pageSize, int pageNumber, string sortParams, CustomerSearch model) {
            var customers = model.Build(context.Customers.Include(x => x.CreatedSubject).Include(x => x.LastModifiedSubject).AsNoTracking());
            var result = new PagedList<Customer> {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = await customers.CountAsync().ConfigureAwait(false),
                Items = new List<Customer>(),
            };

            customers = customers.ToSortedQuery(sortParams);
            result.Items = await customers.ToPagedQuery(pageNumber, pageSize).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public Customer Add(Customer customer) {
            var entity = context.Customers.Add(customer);
            return entity.Entity;
        }

        public Task<Customer> GetAsync(Guid id) {
            return context.Customers
                .Include(x => x.CreatedSubject)
                .Include(x => x.LastModifiedSubject)
                .FirstOrDefaultAsync(o => o.CustomerResourceId == id);
        }
    }
}
