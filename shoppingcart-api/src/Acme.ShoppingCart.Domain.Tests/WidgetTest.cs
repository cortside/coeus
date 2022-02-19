using Xunit;

namespace Acme.ShoppingCart.Domain.Tests {
    public class WidgetTest {
        [Fact]
        public void Foo() {
            // Arrange
            var widget = new Widget();

            // Act
            widget.WidgetId = 1;

            // Assert
            Assert.Equal(1, widget.WidgetId);
        }
    }
}
