using Acme.ShoppingCart.Data;
using Cortside.DomainEvent;
using Moq;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.DomainService.Tests {
    public class ShoppingCarterviceTest : DomainServiceTest<ICustomerService> {
        private readonly DatabaseContext databaseContext;
        private readonly Mock<IDomainEventPublisher> domainEventPublisherMock;
        private readonly ITestOutputHelper testOutputHelper;

        public ShoppingCarterviceTest(ITestOutputHelper testOutputHelper) : base() {
            databaseContext = GetDatabaseContext();
            domainEventPublisherMock = testFixture.Mock<IDomainEventPublisher>();
            this.testOutputHelper = testOutputHelper;
        }
    }
}
