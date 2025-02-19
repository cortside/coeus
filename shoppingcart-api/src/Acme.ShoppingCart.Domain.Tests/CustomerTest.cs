using Acme.ShoppingCart.TestUtilities;
using Shouldly;
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
            customer.FirstName.ShouldBe("elmer");
            customer.LastName.ShouldBe("fudd");
            customer.Email.ShouldBe("elmer@fudd.org");
        }
    }
}
