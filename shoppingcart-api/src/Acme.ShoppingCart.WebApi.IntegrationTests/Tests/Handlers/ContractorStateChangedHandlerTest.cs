//using System;
//using System.Linq;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using Acme.ShoppingCart.Data;
//using Cortside.Common.Threading;
//using Cortside.DomainEvent;
//using Cortside.DomainEvent.Stub;
//using FluentAssertions;
//using Microsoft.Extensions.DependencyInjection;
//using Xunit;

//namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests.Handlers {
//    public class ContractorStateChangedHandlerTest : IClassFixture<IntegrationTestFactory<Startup>> {
//        private readonly HttpClient client;
//        private readonly IStubBroker broker;
//        private readonly IDomainEventPublisher publisher;
//        private DatabaseContext db;
//        private readonly IntegrationTestFactory<Startup> fixture;

//        public ContractorStateChangedHandlerTest(IntegrationTestFactory<Startup> fixture) {
//            client = fixture.CreateAuthorizedClient("api");

//            broker = fixture.Services.GetService<IStubBroker>();
//            publisher = fixture.Services.GetService<IDomainEventPublisher>();
//            db = fixture.NewScopedDbContext();
//            this.fixture = fixture;
//        }

//        [Fact]
//        public async Task ShouldSyncContractorAsync() {
//            // arrage
//            var message = new ContractorStateChangedEvent() {
//                ContractorId = 1,
//                ContractorName = "Test",
//                ContractorNumber = 1,
//                SponsorId = 1,
//                SponsorNumber = 1,
//                Timestamp = DateTime.UtcNow
//            };
//            await publisher.PublishAsync(message);

//            // act
//            await AsyncUtil.WaitUntilAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token, () => !broker.HasItems).ConfigureAwait(false);

//            // assert
//            db = fixture.NewScopedDbContext();
//            var contractor = db.Contractors.FirstOrDefault(x => x.ContractorId == message.ContractorId);
//            Assert.NotNull(contractor);
//            contractor.ContractorNumber.Should().Be(message.ContractorNumber);
//            contractor.ContractorName.Should().Be(message.ContractorName);
//        }
//    }
//}
