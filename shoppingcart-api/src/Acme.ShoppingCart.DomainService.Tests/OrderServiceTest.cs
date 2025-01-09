using System.Threading.Tasks;
using Acme.ShoppingCart.CatalogApi;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.TestUtilities;
using Cortside.DomainEvent.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Acme.ShoppingCart.DomainService.Tests {
    public class OrderServiceTest : DomainServiceTest<IOrderService> {
        private readonly DatabaseContext databaseContext;

        public OrderServiceTest() : base() {
            databaseContext = GetDatabaseContext();
            var publisher = new Mock<IDomainEventOutboxPublisher>();
            var orderRepository = new OrderRepository(databaseContext);
            Service = new OrderService(orderRepository, publisher.Object, NullLogger<OrderService>.Instance, new Mock<ICatalogClient>().Object);

            var customer = EntityBuilder.GetCustomerEntity();
            databaseContext.Customers.Add(customer);
            databaseContext.SaveChanges(true);
        }

        [Fact]
        public async Task ShouldCreateOrderAsync() {
            // Arrange
            var customer = await databaseContext.Customers.FirstAsync();
            var order = DtoBuilder.GetCreateOrderDto();

            // Act
            await Service.CreateOrderAsync(customer, order);
            await databaseContext.SaveChangesAsync();

            // Assert
            Assert.True(await databaseContext.Orders.AnyAsync(x => x.Address.Street == order.Address.Street));
        }
    }
}
