using System.Net;
using System.Threading.Tasks;
using Cortside.RestApiClient;
using FluentAssertions;
using RestSharp;
using Xunit;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class SettingsTest : IClassFixture<WebApiApplicationFactory> {
        private readonly WebApiApplicationFactory webApi;
        private readonly ITestOutputHelper output;
        private readonly RestApiClient client;

        public SettingsTest(WebApiApplicationFactory webApi, ITestOutputHelper output) {
            this.webApi = webApi;
            this.output = output;
            client = webApi.CreateRestApiClient(output);
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
