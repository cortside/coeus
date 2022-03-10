using System;
using System.Linq;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.Dto;
using Cortside.DomainEvent;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.DomainService.Tests {
    public class CustomerServiceTest : DomainServiceTest<ICustomerService> {
        private readonly DatabaseContext databaseContext;

        public CustomerServiceTest(ITestOutputHelper testOutputHelper) : base() {
            databaseContext = GetDatabaseContext();
        }

        [Fact]
        public async Task ShouldCreateCustomer() {
            // Arrange
            var dto = new CustomerDto() {
                FirstName = Guid.NewGuid().ToString()
            };

            var publisher = new Mock<IDomainEventOutboxPublisher>();
            var customerRepository = new CustomerRepository(databaseContext);
            var service = new CustomerService(customerRepository, publisher.Object, NullLogger<CustomerService>.Instance);

            // Act
            await service.CreateCustomerAsync(dto).ConfigureAwait(false);
            await databaseContext.SaveChangesAsync();

            // Assert
            Assert.True(databaseContext.Customers.Any(x => x.FirstName == dto.FirstName));
        }
    }
}
