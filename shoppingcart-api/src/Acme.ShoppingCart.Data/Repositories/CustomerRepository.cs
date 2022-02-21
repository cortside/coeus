using System;
using System.Collections.Generic;
using System.Linq;
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
            var customers = model.Build(context.Customers.AsNoTracking());
            var result = new PagedList<Customer> {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = await customers.CountAsync().ConfigureAwait(false),
                Items = new List<Customer>(),
            };

            customers = customers.ToSortedQuery(sortParams);
            result.Items = await customers.ToPagedQuery(pageNumber, pageSize).ToListAsync();

            return result;
        }

        public async Task<Order> AddAsync(Order order) {
            var entity = await context.Orders.AddAsync(order).ConfigureAwait(false);
            return entity.Entity;
        }

        public async Task<Order> GetAsync(Guid id) {
            var order = await context
                                .Orders
                                .Include(x => x.Address)
                                .Include(x => x.Customer)
                                .FirstOrDefaultAsync(o => o.OrderResourceId == id);
            if (order == null) {
                order = context
                            .Orders
                            .Local
                            .FirstOrDefault(o => o.OrderResourceId == id);
            }
            if (order != null) {
                await context.Entry(order)
                    .Collection(i => i.Items).LoadAsync();
            }

            return order;
        }
    }
}
