using Acme.ShoppingCart.Domain.Entities;
using Xunit;

namespace Acme.ShoppingCart.Domain.Tests {
    public class CustomerTest {
        [Fact]
        public void Foo() {
            // Arrange
            var customer = new Customer("foo", "bar", "foo@baz.com", null);

            // Act
            customer.Update("elmer", "fudd", "elmer@fudd.org");

            // Assert
            Assert.Equal("elmer", customer.FirstName);
        }
    }
}
