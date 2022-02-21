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
                .Include(x => x.LastModifiedSubject);
            //.AsTracking();   // TODO: i can't seem to get back to IQueryable from IIncludeQuerable

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
                .FirstOrDefaultAsync(o => o.OrderResourceId == id);

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

            return order;
        }

        public Task<Order> UpdateAsync(Order order) {
            context.Entry(order).State = EntityState.Modified;
            return Task.FromResult(order);
        }
    }
}
