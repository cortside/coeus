using System;
using System.Linq;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Dto;
using Cortside.DomainEvent;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.DomainService.Tests {
    public class WidgetServiceTest : DomainServiceTest<IWidgetService> {
        private readonly DatabaseContext databaseContext;
        private readonly Mock<IDomainEventPublisher> domainEventPublisherMock;
        private readonly ITestOutputHelper testOutputHelper;

        public WidgetServiceTest(ITestOutputHelper testOutputHelper) : base() {
            databaseContext = GetDatabaseContext();
            domainEventPublisherMock = testFixture.Mock<IDomainEventPublisher>();
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ShouldCreateWidget() {
            // Arrange
            var dto = new WidgetDto() {
                Text = Guid.NewGuid().ToString()
            };

            var publisher = new Mock<IDomainEventOutboxPublisher>();
            var service = new WidgetService(databaseContext, publisher.Object, NullLogger<WidgetService>.Instance);

            // Act
            await service.CreateWidgetAsync(dto);

            // Assert
            Assert.True(databaseContext.Widgets.Any(x => x.Text == dto.Text));
        }
    }
}
