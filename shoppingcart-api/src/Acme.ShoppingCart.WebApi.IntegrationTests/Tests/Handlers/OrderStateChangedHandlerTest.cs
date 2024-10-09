using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.TestUtilities;
using Cortside.Common.Threading;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Stub;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests.Handlers {
    public class OrderStateChangedHandlerTest : IClassFixture<IntegrationFixture> {
        private readonly IStubBroker broker;
        private readonly IDomainEventPublisher publisher;
        private readonly IntegrationFixture fixture;

        public OrderStateChangedHandlerTest(IntegrationFixture fixture) {
            broker = fixture.Services.GetService<IStubBroker>();
            publisher = fixture.Services.GetService<IDomainEventPublisher>();
            this.fixture = fixture;
        }

        [Fact]
        public async Task ShouldSendNotificationAsync() {
            //arrange
            var db = fixture.NewScopedDbContext<DatabaseContext>();
            var order = EntityBuilder.GetOrderEntity();
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var message = new OrderStateChangedEvent() {
                OrderResourceId = order.OrderResourceId,
                Timestamp = DateTime.UtcNow
            };

            var correlationId = Guid.NewGuid().ToString();
            await publisher.PublishAsync(message, correlationId);

            // act
            await ReceiveAndWaitAsync(correlationId, 10);

            // assert
            db = fixture.NewScopedDbContext<DatabaseContext>();
            var entity = db.Orders.FirstOrDefault(x => x.OrderResourceId == message.OrderResourceId);
            Assert.NotNull(entity);
            Assert.NotNull(entity.LastNotified);
        }

        private async Task ReceiveAndWaitAsync(string correlationId, int timeout) {
            await AsyncUtil.WaitUntilAsync(new CancellationTokenSource(TimeSpan.FromSeconds(timeout)).Token, () => !broker.HasItems);

            var message = broker.AcceptedItems.FirstOrDefault(x => x.Properties.CorrelationId == correlationId);
            Assert.NotNull(message);
        }
    }
}
