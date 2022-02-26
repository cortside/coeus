using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Paging;
using Acme.ShoppingCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Acme.ShoppingCart.Data.Repositories {
    public class OrderRepository : IOrderRepository {
        private readonly DatabaseContext context;

        public IUnitOfWork UnitOfWork {
            get {
                return context;
            }
        }

        public OrderRepository(DatabaseContext context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PagedList<Order>> SearchAsync(int pageSize, int pageNumber, string sortParams, IOrderSearch model) {
            var orders = (IQueryable<Order>)context.Orders
                .Include(x => x.Address)
                .Include(x => x.Customer)
                .Include(x => x.CreatedSubject)
                .Include(x => x.LastModifiedSubject)
                .Include(x => x.Items)
                    .ThenInclude(x => x.CreatedSubject)
                .Include(x => x.Items)
                    .ThenInclude(x => x.LastModifiedSubject);

            var tx = context.Database.CurrentTransaction?.GetDbTransaction();
            if (tx != null && tx.IsolationLevel == IsolationLevel.ReadUncommitted) {
                orders.AsNoTrackingWithIdentityResolution();
            }

            orders = model.Build(orders);
            var result = new PagedList<Order> {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = await orders.CountAsync().ConfigureAwait(false),
                Items = new List<Order>(),
            };

            orders = orders.ToSortedQuery(sortParams);
            result.Items = await orders.ToPagedQuery(pageNumber, pageSize).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public async Task<Order> AddAsync(Order order) {
            var entity = await context.Orders.AddAsync(order).ConfigureAwait(false);
            return entity.Entity;
        }

        public async Task<Order> GetAsync(Guid id) {
            var order = await context.Orders
                .Include(x => x.Address)
                .Include(x => x.Customer)
                .Include(x => x.CreatedSubject)
                .Include(x => x.LastModifiedSubject)
                .Include(x => x.Items)
                    .ThenInclude(x => x.CreatedSubject)
                .Include(x => x.Items)
                    .ThenInclude(x => x.LastModifiedSubject)
                .FirstOrDefaultAsync(o => o.OrderResourceId == id);

            return order;
        }
    }
}
