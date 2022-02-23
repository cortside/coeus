using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.WebApi.Models.Requests;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class OrderTest : IClassFixture<IntegrationTestFactory<Startup>> {
        private readonly IntegrationTestFactory<Startup> fixture;
        private readonly HttpClient testServerClient;

        public OrderTest(IntegrationTestFactory<Startup> fixture) {
            this.fixture = fixture;
            testServerClient = fixture.CreateAuthorizedClient("api");
        }

        [Fact]
        public async Task ShouldCreateOrderAsync() {
            //arrange
            var request = new Models.Requests.CreateCustomerModel() {
                FirstName = Guid.NewGuid().ToString(),
                LastName = "last",
                Email = "email"
            };
            var requestBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var orderRequest = new CreateOrderModel() {
                Address = new Models.AddressModel() {
                    Street = "123 Main",
                    City = "Salt Lake City",
                    State = "UT",
                    Country = "USA",
                    ZipCode = "84009"
                },
                Items = new System.Collections.Generic.List<CreateOrderItemModel>() {
                     new CreateOrderItemModel() { Sku = "123", Quantity= 1 }
                 }
            };

            //act
            var response = await testServerClient.PostAsync("/api/v1/customers", requestBody).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var customer = JsonConvert.DeserializeObject<Models.Responses.CustomerModel>(content);
            orderRequest.CustomerResourceId = customer.CustomerResourceId;
            var orderBody = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            var orderResponse = await testServerClient.PostAsync("/api/v1/orders", orderBody).ConfigureAwait(false);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var orderContent = await orderResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            orderResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task ShouldGetOrderAsync() {
            //arrange
            var db = fixture.NewScopedDbContext();
            var customer = db.Customers.First();
            var order = new Order(customer, "", "", "", "", "");
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            //act
            var response = await testServerClient.GetAsync($"api/v1/orders/{order.OrderResourceId}").ConfigureAwait(false);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ShouldGetPagedOrdersAsync() {
            //arrange
            var db = fixture.NewScopedDbContext();
            var customer = db.Customers.First();
            for (int i = 0; i < 10; i++) {
                var order = new Order(customer, "", "", "", "", "");
                db.Orders.Add(order);
            }
            await db.SaveChangesAsync();

            //act
            var response = await testServerClient.GetAsync($"api/v1/orders?pageSize=5&page=1").ConfigureAwait(false);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
