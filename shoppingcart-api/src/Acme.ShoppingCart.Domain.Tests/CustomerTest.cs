using System;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.TestUtilities;
using KellermanSoftware.CompareNetObjects;
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

        [Fact]
        public void CompareWithPropertyExclusions() {
            var customer = new Customer("elmer", "fudd", "elmer@fudd.org");
            customer.CreatedDate = DateTime.Now.AddSeconds(-1);
            var customer2 = new Customer("elmer", "fudd", "elmer@fudd.org");
            customer2.CreatedDate = DateTime.Now;

            CompareLogic compare = new CompareLogic();
            compare.Config.IgnoreProperty<Customer>(x => x.CustomerResourceId);
            compare.Config.IgnoreProperty<Customer>(x => x.CreatedDate);
            ComparisonResult result = compare.Compare(customer, customer2);

            result.AreEqual.ShouldBeTrue(result.DifferencesString);
        }
    }
}
