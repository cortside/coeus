using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Acme.ShoppingCart.WebApi.Models.Responses;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class OrderTest : IClassFixture<IntegrationTestFactory<Startup>> {
        private readonly IntegrationTestFactory<Startup> fixture;
        private readonly HttpClient client;

        public OrderTest(IntegrationTestFactory<Startup> fixture) {
            this.fixture = fixture;
            client = fixture.CreateAuthorizedClient("api");
        }

        [Fact]
        public async Task ShouldCreateOrderAsync() {
            //arrange
            var orderRequest = new CreateOrderModel() {
                Customer = new CreateCustomerModel() {
                    FirstName = "Elmer",
                    LastName = "Fudd",
                    Email = "elmer.fudd@gmail.com"
                },
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
            var orderBody = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            var orderResponse = await client.PostAsync("/api/v1/orders", orderBody).ConfigureAwait(false);

            //assert
            var orderContent = await orderResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            orderResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.Should().NotBeEmpty();
            order.Items.Count.Should().Be(1);
        }

        [Fact]
        public async Task ShouldCreateCustomerOrderAsync() {
            //arrange
            var request = new CreateCustomerModel() {
                FirstName = Guid.NewGuid().ToString(),
                LastName = "last",
                Email = "email@gmail.com"
            };
            var requestBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var orderRequest = new CreateCustomerOrderModel() {
                Address = new Models.AddressModel() {
                    Street = "123 Main",
                    City = "Salt Lake City",
                    State = "UT",
                    Country = "USA",
                    ZipCode = "84009"
                },
                Items = new List<CreateOrderItemModel>() {
                     new CreateOrderItemModel() { Sku = "123", Quantity= 1 }
                 }
            };

            //act
            var response = await client.PostAsync("/api/v1/customers", requestBody).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var customer = JsonConvert.DeserializeObject<Models.Responses.CustomerModel>(content);

            var orderBody = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            var orderResponse = await client.PostAsync($"/api/v1/customers/{customer.CustomerResourceId}/orders", orderBody).ConfigureAwait(false);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var orderContent = await orderResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            orderResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.Should().Be(customer.CustomerResourceId);
            order.Items.Count.Should().Be(1);
        }

        [Fact]
        public async Task ShouldGetOrderAsync() {
            //arrange
            var db = fixture.NewScopedDbContext();
            var customer = db.Customers.First();
            var order = new Order(customer, "", "", "", "", "");
            db.Orders.Add(order);
            await db.SaveChangesAsync().ConfigureAwait(false);

            //act
            var response = await client.GetAsync($"api/v1/orders/{order.OrderResourceId}").ConfigureAwait(false);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ShouldAddOrderItemAsync() {
            //arrange
            var db = fixture.NewScopedDbContext();
            var customer = db.Customers.First();
            var orderRequest = new Order(customer, "", "", "", "", "");
            db.Orders.Add(orderRequest);
            await db.SaveChangesAsync().ConfigureAwait(false);

            //act
            var orderResponse = await client.GetAsync($"api/v1/orders/{orderRequest.OrderResourceId}").ConfigureAwait(false);

            //assert
            var orderContent = await orderResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.Should().NotBeEmpty();
            order.OrderResourceId.Should().NotBeEmpty();
            order.Items.Count.Should().Be(0);
            order.Status.Should().Be(Models.Enumerations.OrderStatus.Created);

            // act
            var itemRequest = new CreateOrderItemModel() { Sku = "123", Quantity = 1 };
            var orderBody = new StringContent(JsonConvert.SerializeObject(itemRequest), Encoding.UTF8, "application/json");
            orderResponse = await client.PostAsync($"api/v1/orders/{orderRequest.OrderResourceId}/items", orderBody).ConfigureAwait(false);

            //assert
            orderContent = await orderResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Contains(orderRequest.OrderResourceId.ToString(), orderContent);

            //act
            orderResponse = await client.GetAsync($"api/v1/orders/{orderRequest.OrderResourceId}").ConfigureAwait(false);

            //assert
            orderContent = await orderResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.Should().NotBeEmpty();
            order.OrderResourceId.Should().NotBeEmpty();
            order.Items.Count.Should().Be(1);
            order.Status.Should().Be(Models.Enumerations.OrderStatus.Created);
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
            await db.SaveChangesAsync().ConfigureAwait(false);

            //act
            var response = await client.GetAsync("api/v1/orders?pageSize=5&page=1").ConfigureAwait(false);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ShouldUpdateOrderAsync() {
            //arrange
            var orderRequest = new CreateOrderModel() {
                Customer = new CreateCustomerModel() {
                    FirstName = "Elmer",
                    LastName = "Fudd",
                    Email = "elmer.fudd@gmail.com"
                },
                Address = new Models.AddressModel() {
                    Street = "123 Main",
                    City = "Salt Lake City",
                    State = "UT",
                    Country = "USA",
                    ZipCode = "84009"
                },
                Items = new System.Collections.Generic.List<CreateOrderItemModel>() {
                     new CreateOrderItemModel() { Sku = "123", Quantity= 1 },
                     new CreateOrderItemModel() { Sku = "456", Quantity= 2 }
                 }
            };

            //act
            var orderBody = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            var orderResponse = await client.PostAsync("/api/v1/orders", orderBody).ConfigureAwait(false);

            //assert
            var orderContent = await orderResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            orderResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.Should().NotBeEmpty();
            order.OrderResourceId.Should().NotBeEmpty();
            order.Items.Count.Should().Be(2);

            // act
            orderRequest.Items.RemoveAt(0);
            orderRequest.Items.Add(new CreateOrderItemModel() { Sku = "789", Quantity = 3 });
            orderBody = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            orderResponse = await client.PutAsync($"/api/v1/orders/{order.OrderResourceId}", orderBody).ConfigureAwait(false);
            orderContent = await orderResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            orderResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.Should().NotBeEmpty();
            order.Items.Count.Should().Be(2);
            order.Items.Where(x => x.Sku == "789").Should().NotBeEmpty();
        }
    }
}
