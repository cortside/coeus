using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Cortside.AspNetCore.Common.Paging;
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
                Email = "email@gmail.com"
            };

            var requestBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            //act
            var response = await testServerClient.PostAsync("/api/v1/customers", requestBody);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var content = await response.Content.ReadAsStringAsync();
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
            var response = await testServerClient.GetAsync($"api/v1/customers/{id}");

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ShouldSearchCustomersAsync() {
            //arrange
            var db = fixture.NewScopedDbContext();
            var id = db.Customers.First();

            var model = new Models.Requests.CustomerSearchModel() {
                FirstName = id.FirstName
            };

            var requestBody = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            //act
            var response = await testServerClient.PostAsync("/api/v1/customers/search", requestBody);

            //assert
            // testserver httpclient does not follow redirects and no current way to make that happen, so have to handle this manually
            response.StatusCode.Should().Be(HttpStatusCode.SeeOther);
            var location = response.Headers.FirstOrDefault(x => x.Key == "Location").Value.First();
            location.Should().NotBeNull();

            var customersResponse = await testServerClient.GetAsync(location);
            customersResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await customersResponse.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<PagedList<CustomerModel>>(content);
            customers.Items.Exists(x => x.CustomerResourceId == id.CustomerResourceId).Should().BeTrue();
        }
    }
}
