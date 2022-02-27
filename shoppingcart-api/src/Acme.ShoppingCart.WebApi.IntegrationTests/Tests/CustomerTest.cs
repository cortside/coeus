using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class CustomerTest : IClassFixture<IntegrationTestFactory<Startup>> {
        private readonly IntegrationTestFactory<Startup> fixture;
        private readonly HttpClient testServerClient;

        public CustomerTest(IntegrationTestFactory<Startup> fixture) {
            this.fixture = fixture;
            testServerClient = fixture.CreateAuthorizedClient("api");
        }

        [Fact]
        public async Task ShouldCreateCustomerAsync() {
            //arrange
            var request = new Models.Requests.CreateCustomerModel() {
                FirstName = Guid.NewGuid().ToString(),
                LastName = "last",
                Email = "email"
            };

            var requestBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            //act
            var response = await testServerClient.PostAsync("/api/v1/customers", requestBody).ConfigureAwait(false);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var customer = JsonConvert.DeserializeObject<Models.Responses.CustomerModel>(content);
            Assert.Equal(request.FirstName, customer.FirstName);
            Assert.Equal(request.LastName, customer.LastName);
            Assert.Equal(request.Email, customer.Email);
        }

        [Fact]
        public async Task ShouldGetCustomerAsync() {
            //arrange
            var db = fixture.NewScopedDbContext();
            var id = db.Customers.First().CustomerResourceId;

            //act
            var response = await testServerClient.GetAsync($"api/v1/customers/{id}").ConfigureAwait(false);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
