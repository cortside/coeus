using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Cortside.AspNetCore.Common.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class CustomerTypeTest : IClassFixture<IntegrationTestFactory<Startup>> {
        private readonly IntegrationTestFactory<Startup> fixture;
        private readonly HttpClient testServerClient;

        public CustomerTypeTest(IntegrationTestFactory<Startup> fixture) {
            this.fixture = fixture;
            testServerClient = fixture.CreateAuthorizedClient("api");
        }

        [Fact]
        public async Task ShouldGetCustomerTypesAsync() {
            //arrange
            var db = fixture.NewScopedDbContext();
            var count = db.CustomerTypes.Count();

            //act
            var response = await testServerClient.GetAsync("api/v1/customertypes").ConfigureAwait(false);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var results = JsonConvert.DeserializeObject<ListResult<CustomerTypeModel>>(content);
            Assert.Equal(count, results.Results.Count);
        }
    }
}
