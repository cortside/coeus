using Newtonsoft.Json;
using PolicyServer.Mocks.Models;
using System;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PolicyServer.Mocks
{
    public class CatalogMock : IWireMockBuilder
    {
        public void Configure(WireMockServer server)
        {
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
                        .WithBody(r => JsonConvert.SerializeObject(new CatalogItem()
                        {
                            ItemId = Guid.NewGuid(),
                            Name = "",
                            Sku = r.PathSegments[3],
                            UnitPrice = new decimal(rnd.NextDouble())
                        }))
                );
        }
    }
}
