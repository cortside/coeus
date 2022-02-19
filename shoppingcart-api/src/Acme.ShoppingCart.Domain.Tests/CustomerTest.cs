using Acme.ShoppingCart.Domain.Entities;
using Xunit;

namespace Acme.ShoppingCart.Domain.Tests {
    public class CustomerTest {
        [Fact]
        public void Foo() {
            // Arrange
            var customer = new Customer();

            // Act
            customer.CustomerId = 1;

            // Assert
            Assert.Equal(1, customer.CustomerId);
        }
    }
}
