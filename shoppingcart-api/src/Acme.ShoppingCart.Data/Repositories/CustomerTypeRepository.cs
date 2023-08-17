using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.AspNetCore.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.Data.Repositories {
    public class CustomerTypeRepository : ICustomerTypeRepository {
        private readonly IDatabaseContext context;

        public CustomerTypeRepository(IDatabaseContext context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ListResult<CustomerType>> GetCustomerTypesAsync() {
            var list = await context.CustomerTypes.ToListAsync();
            return new ListResult<CustomerType>(list);
        }
    }
}
