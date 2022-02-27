using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.Data.Repositories {
    public class CustomerRepository : ICustomerRepository {
        private readonly DatabaseContext context;

        public CustomerRepository(DatabaseContext context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => context;

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

        public async Task<Customer> AddAsync(Customer customer) {
            var entity = await context.Customers.AddAsync(customer).ConfigureAwait(false);
            return entity.Entity;
        }

        public async Task<Customer> GetAsync(Guid id) {
            var entity = await context
                                .Customers
                                .Include(x => x.CreatedSubject)
                                .Include(x => x.LastModifiedSubject)
                                .FirstOrDefaultAsync(o => o.CustomerResourceId == id).ConfigureAwait(false);

            //if (order == null) {
            //    order = context
            //                .Orders
            //                .Local
            //                .FirstOrDefault(o => o.OrderResourceId == id);
            //}
            //if (order != null) {
            //    await context.Entry(order)
            //        .Collection(i => i.Items).LoadAsync();
            //}

            return entity;
        }
    }
}
