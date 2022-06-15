using System;
using Acme.ShoppingCart.UserClient.Models.Responses;
using Cortside.MockServer;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Mocks {
    public class CatalogMock : IMockHttpServerBuilder {
        public void Configure(WireMockServer server) {
            var rnd = new Random();

            server
                .Given(
                    Request.Create().WithPath("/api/v1/items/*")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(r => JsonConvert.SerializeObject(new CatalogItem() {
                            ItemId = Guid.NewGuid(),
                            Name = $"Item {r.PathSegments[3]}",
                            Sku = r.PathSegments[3],
                            UnitPrice = new decimal(rnd.Next(10000) / 100.0)
                        }))
                );
        }
    }
}
