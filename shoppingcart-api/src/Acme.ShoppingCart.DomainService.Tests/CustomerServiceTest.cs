using System.Linq;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Data.Repositories;
using Acme.ShoppingCart.TestUtilities;
using Cortside.DomainEvent.EntityFramework;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Acme.ShoppingCart.DomainService.Tests {
    public class CustomerServiceTest : DomainServiceTest<CustomerService> {
        private readonly DatabaseContext databaseContext;

        public CustomerServiceTest() : base() {
            databaseContext = GetDatabaseContext();

            var publisher = new Mock<IDomainEventOutboxPublisher>();
            var customerRepository = new CustomerRepository(databaseContext);
            Service = new CustomerService(customerRepository, publisher.Object, NullLogger<CustomerService>.Instance);
        }

        [Fact]
        public async Task ShouldCreateCustomerAsync() {
            // Arrange
            var dto = DtoBuilder.GetUpdateCustomerDto();

            // Act
            await Service.CreateCustomerAsync(dto);
            await databaseContext.SaveChangesAsync();

            // Assert
            Assert.True(databaseContext.Customers.Any(x => x.FirstName == dto.FirstName));
        }
    }
}
