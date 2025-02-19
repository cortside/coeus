using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.TestUtilities;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Acme.ShoppingCart.WebApi.Models.Responses;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using OrderStatus = Acme.ShoppingCart.WebApi.Models.Enumerations.OrderStatus;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class OrderTest : IClassFixture<IntegrationFixture> {
        private readonly IntegrationFixture fixture;
        private readonly HttpClient client;

        public OrderTest(IntegrationFixture fixture) {
            this.fixture = fixture;
            client = fixture.CreateAuthorizedClient("api");
        }

        [Fact]
        public async Task ShouldCreateOrderAsync() {
            //arrange
            var orderRequest = ModelBuilder.GetCreateOrderModel();

            //act
            var orderBody = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            var orderResponse = await client.PostAsync("/api/v1/orders", orderBody);

            //assert
            var orderContent = await orderResponse.Content.ReadAsStringAsync();
            orderResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.ShouldNotBe(Guid.Empty);
            order.Items.Count.ShouldBe(1);
        }

        [Fact]
        public async Task ShouldCreateCustomerOrderAsync() {
            //arrange
            var request = ModelBuilder.GetUpdateCustomerModel();
            var requestBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var orderRequest = ModelBuilder.GetCreateOrderModel();

            //act
            var response = await client.PostAsync("/api/v1/customers", requestBody);
            var content = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Models.Responses.CustomerModel>(content);

            var orderBody = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            var orderResponse = await client.PostAsync($"/api/v1/customers/{customer.CustomerResourceId}/orders", orderBody);

            //assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var orderContent = await orderResponse.Content.ReadAsStringAsync();
            orderResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.ShouldBe(customer.CustomerResourceId);
            order.Items.Count.ShouldBe(1);
        }

        [Fact]
        public async Task ShouldGetOrderAsync() {
            //arrange
            var db = fixture.NewScopedDbContext<DatabaseContext>();
            var order = EntityBuilder.GetOrderEntity();
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            //act
            var response = await client.GetAsync($"api/v1/orders/{order.OrderResourceId}");

            //assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ShouldAddOrderItemAsync() {
            //arrange
            var db = fixture.NewScopedDbContext<DatabaseContext>();
            var orderEntity = EntityBuilder.GetOrderEntity();
            db.Orders.Add(orderEntity);
            await db.SaveChangesAsync();

            //act
            var orderResponse = await client.GetAsync($"api/v1/orders/{orderEntity.OrderResourceId}");

            //assert
            var orderContent = await orderResponse.Content.ReadAsStringAsync();
            orderResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.ShouldNotBe(Guid.Empty);
            order.OrderResourceId.ShouldNotBe(Guid.Empty);
            order.Items.Count.ShouldBe(0);
            order.Status.ShouldBe(OrderStatus.Created);

            // act
            var itemRequest = ModelBuilder.GetCreateOrderItemModel();
            var orderBody = new StringContent(JsonConvert.SerializeObject(itemRequest), Encoding.UTF8, "application/json");
            orderResponse = await client.PostAsync($"api/v1/orders/{orderEntity.OrderResourceId}/items", orderBody);

            //assert
            orderContent = await orderResponse.Content.ReadAsStringAsync();
            orderResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            Assert.Contains(orderEntity.OrderResourceId.ToString(), orderContent);

            //act
            orderResponse = await client.GetAsync($"api/v1/orders/{orderEntity.OrderResourceId}");

            //assert
            orderContent = await orderResponse.Content.ReadAsStringAsync();
            orderResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.ShouldNotBe(Guid.Empty);
            order.OrderResourceId.ShouldNotBe(Guid.Empty);
            order.Items.Count.ShouldBe(1);
            order.Status.ShouldBe(OrderStatus.Created);
        }

        [Fact]
        public async Task ShouldGetPagedOrdersAsync() {
            //arrange
            var db = fixture.NewScopedDbContext<DatabaseContext>();
            for (int i = 0; i < 10; i++) {
                var order = EntityBuilder.GetOrderEntity();
                db.Orders.Add(order);
            }
            await db.SaveChangesAsync();

            //act
            var response = await client.GetAsync("api/v1/orders?pageSize=5&page=1");

            //assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ShouldUpdateOrderAsync() {
            //arrange
            var orderRequest = ModelBuilder.GetCreateOrderModel();
            orderRequest.Items.Add(ModelBuilder.GetCreateOrderItemModel());

            //act
            var orderBody = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            var orderResponse = await client.PostAsync("/api/v1/orders", orderBody);

            //assert
            var orderContent = await orderResponse.Content.ReadAsStringAsync();
            orderResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.ShouldNotBe(Guid.Empty);
            order.OrderResourceId.ShouldNotBe(Guid.Empty);
            order.Items.Count.ShouldBe(2);

            // act
            orderRequest.Items.RemoveAt(0);
            orderRequest.Items.Add(new CreateOrderItemModel() { Sku = "789", Quantity = 3 });
            orderBody = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            orderResponse = await client.PutAsync($"/api/v1/orders/{order.OrderResourceId}", orderBody);
            orderContent = await orderResponse.Content.ReadAsStringAsync();
            orderResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            order = JsonConvert.DeserializeObject<OrderModel>(orderContent);
            order.Customer.CustomerResourceId.ShouldNotBe(Guid.Empty);
            order.Items.Count.ShouldBe(2);
            order.Items.Where(x => x.Sku == "789").ShouldNotBeEmpty();
        }

        [Fact]
        public async Task ShouldCancelOrderAsync() {
            //arrange
            var orderRequest = ModelBuilder.GetCreateOrderModel();
            orderRequest.Items.Add(ModelBuilder.GetCreateOrderItemModel());

            //act
            var orderBody = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            var orderResponse = await client.PostAsync("/api/v1/orders", orderBody);

            //assert
            var orderContent = await orderResponse.Content.ReadAsStringAsync();
            orderResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var order = JsonConvert.DeserializeObject<OrderModel>(orderContent);

            // act
            orderResponse = await client.PostAsync($"/api/v1/orders/{order.OrderResourceId}/cancel", orderBody);

            //assert
            orderResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            //act
            var response = await client.GetAsync($"api/v1/orders/{order.OrderResourceId}");

            //assert
            orderContent = await response.Content.ReadAsStringAsync();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            order = JsonConvert.DeserializeObject<OrderModel>(orderContent);

            order.Status.ShouldBe(OrderStatus.Cancelled);
        }
    }
}
