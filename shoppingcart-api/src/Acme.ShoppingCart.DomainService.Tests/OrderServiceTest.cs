using System;
using System.Linq;
using System.Threading.Tasks;
using Acme.ShoppingCart.CatalogApi;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;
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
            service = new OrderService(orderRepository, publisher.Object, NullLogger<OrderService>.Instance, new Mock<ICatalogClient>().Object);

            var name = Guid.NewGuid().ToString();
            var customer = new Customer(name, name, name + "@gmail.com");
            databaseContext.Customers.Add(customer);
            databaseContext.SaveChanges(true);
        }

        [Fact]
        public async Task ShouldCreateOrderAsync() {
            // Arrange
            var customer = await databaseContext.Customers.FirstAsync();
            var order = new OrderDto() {
                Address = new AddressDto() {
                    Street = Guid.NewGuid().ToString(),
                    City = "Salt Lake City",
                    State = "UT",
                    ZipCode = "84123",
                    Country = "USA"
                },
                Items = []
            };

            // Act
            await service.CreateOrderAsync(customer, order);
            await databaseContext.SaveChangesAsync();

            // Assert
            Assert.True(databaseContext.Orders.Any(x => x.Address.Street == order.Address.Street));
        }
    }
}
