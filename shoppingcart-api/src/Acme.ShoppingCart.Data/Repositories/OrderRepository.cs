using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data.Searches;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Common.Paging;
using Cortside.AspNetCore.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.Data.Repositories {
    public class OrderRepository : IOrderRepository {
        private readonly IDatabaseContext context;

        public OrderRepository(IDatabaseContext context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PagedList<Order>> SearchAsync(IOrderSearch model) {
            var orders = (IQueryable<Order>)context.Orders
                .Include(x => x.Address)
                .Include(x => x.Customer)
                .Include(x => x.CreatedSubject)
                .Include(x => x.LastModifiedSubject)
                .Include(x => x.Items)
                    .ThenInclude(x => x.CreatedSubject)
                .Include(x => x.Items)
                    .ThenInclude(x => x.LastModifiedSubject);

            orders = model.Build(orders);
            var result = new PagedList<Order> {
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
                TotalItems = await orders.CountAsync().ConfigureAwait(false),
                Items = [],
            };

            orders = orders.ToSortedQuery(model.Sort);
            result.Items = await orders.ToPagedQuery(model.PageNumber, model.PageSize).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public async Task<Order> AddAsync(Order order) {
            var entity = await context.Orders.AddAsync(order);
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
                .FirstOrDefaultAsync(o => o.OrderResourceId == id).ConfigureAwait(false);

            return order;
        }

        public void RemoveItems(List<OrderItem> itemsToRemove) {
            context.RemoveRange(itemsToRemove);
        }
    }
}
