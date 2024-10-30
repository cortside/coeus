using System;
using Acme.ShoppingCart.CatalogApi.Models.Responses;
using Cortside.MockServer;
using Cortside.MockServer.Builder;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Mocks {
    public class CatalogMock : IMockHttpMock {
        public void Configure(MockHttpServer server) {
            var rnd = new Random();

            server.WireMockServer
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
                            UnitPrice = new decimal(rnd.Next(10000) / 100.0),
                            Status = (ItemStatus)rnd.Next(4)
                        }))
                );
        }
    }
}
