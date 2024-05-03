using Acme.ShoppingCart.TestUtilities;
using FluentAssertions;
using Xunit;

namespace Acme.ShoppingCart.Domain.Tests {
    public class CustomerTest {
        [Fact]
        public void Foo() {
            // Arrange
            var customer = EntityBuilder.GetCustomerEntity();

            // Act
            customer.Update("elmer", "fudd", "elmer@fudd.org");

            // Assert
            customer.FirstName.Should().Be("elmer");
            customer.LastName.Should().Be("fudd");
            customer.Email.Should().Be("elmer@fudd.org");
        }
    }
}
