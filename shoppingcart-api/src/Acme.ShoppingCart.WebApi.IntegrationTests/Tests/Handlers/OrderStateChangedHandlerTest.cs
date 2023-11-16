using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.Common.Threading;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Stub;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests.Handlers {
    public class OrderStateChangedHandlerTest : IClassFixture<IntegrationTestFactory<Startup>> {
        private readonly IStubBroker broker;
        private readonly IDomainEventPublisher publisher;
        private readonly IntegrationTestFactory<Startup> fixture;

        public OrderStateChangedHandlerTest(IntegrationTestFactory<Startup> fixture) {
            broker = fixture.Services.GetService<IStubBroker>();
            publisher = fixture.Services.GetService<IDomainEventPublisher>();
            this.fixture = fixture;
        }

        [Fact]
        public async Task ShouldSendNotificationAsync() {
            //arrange
            var db = fixture.NewScopedDbContext();
            var customer = db.Customers.First();
            var order = new Order(customer, "", "", "", "", "");
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var message = new OrderStateChangedEvent() {
                OrderResourceId = order.OrderResourceId,
                Timestamp = DateTime.UtcNow
            };
            await publisher.PublishAsync(message);

            // act
            await AsyncUtil.WaitUntilAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token, () => !broker.HasItems);

            // assert
            db = fixture.NewScopedDbContext();
            var entity = db.Orders.FirstOrDefault(x => x.OrderResourceId == message.OrderResourceId);
            Assert.NotNull(entity);
            Assert.NotNull(entity.LastNotified);
        }
    }
}
