using System.Net;
using System.Threading.Tasks;
using Cortside.Common.Testing.Logging.Xunit;
using Cortside.RestApiClient;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using RestSharp;
using Xunit;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class SettingsTest : IClassFixture<IntegrationTestFactory<Startup>> {
        private readonly IntegrationTestFactory<Startup> fixture;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly RestApiClient client;

        public SettingsTest(IntegrationTestFactory<Startup> fixture, ITestOutputHelper testOutputHelper) {
            this.fixture = fixture;
            this.testOutputHelper = testOutputHelper;
            var httpClient = fixture.CreateClient(new WebApplicationFactoryClientOptions {
                AllowAutoRedirect = false
            });
            client = new RestApiClient(new XunitLogger("SettingsTest", testOutputHelper), new HttpContextAccessor(), new RestApiClientOptions(), httpClient);
        }

        [Fact]
        public async Task TestAsync() {
            //arrange
            var request = new RestApiRequest("api/settings", Method.Get);

            //act
            var response = await client.GetAsync(request);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
