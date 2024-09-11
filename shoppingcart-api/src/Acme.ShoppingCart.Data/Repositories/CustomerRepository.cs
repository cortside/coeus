using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Common.Paging;
using Cortside.AspNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.Data.Repositories {
    public class CustomerRepository : ICustomerRepository {
        private readonly IDatabaseContext context;

        public CustomerRepository(IDatabaseContext context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PagedList<Customer>> SearchAsync(CustomerSearch model) {
            var customers = model.Build(context.Customers.Include(x => x.CreatedSubject).Include(x => x.LastModifiedSubject));
            var result = new PagedList<Customer> {
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
                TotalItems = await customers.CountAsync().ConfigureAwait(false),
                Items = [],
            };

            customers = customers.ToSortedQuery(model.Sort);
            result.Items = await customers.ToPagedQuery(model.PageNumber, model.PageSize).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public async Task<Customer> AddAsync(Customer customer) {
            var entity = await context.Customers.AddAsync(customer);
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
