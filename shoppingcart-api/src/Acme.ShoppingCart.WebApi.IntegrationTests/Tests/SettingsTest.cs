using System.Net;
using System.Threading.Tasks;
using Cortside.RestApiClient;
using RestSharp;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class SettingsTest : IClassFixture<IntegrationFixture> {
        private readonly RestApiClient client;
        private readonly IntegrationFixture api;

        public SettingsTest(IntegrationFixture api, ITestOutputHelper output) {
            this.api = api;
            api.TestOutputHelper = output;
            client = api.CreateRestApiClient(output);
        }

        [Fact]
        public async Task TestAsync() {
            //arrange
            var request = new RestApiRequest("api/settings", Method.Get);

            //act
            var response = await client.GetAsync(request);

            //assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.ShouldContain(api.MockServer.Url);
        }
    }
}
