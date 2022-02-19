using System;
using System.Linq;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

        public Task<Order> UpdateAsync(Order order) {
            context.Entry(order).State = EntityState.Modified;
            return Task.FromResult(order);
        }
    }
}
