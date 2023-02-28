using Cortside.MockServer;
using Cortside.MockServer.AccessControl.Models;
using Newtonsoft.Json;
using PolicyServer.Mocks.Models;
using System;
using System.IO;
using System.Linq;
using Cortside.Common.Json;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
using WireMock.Server;

namespace PolicyServer.Mocks
{
    public class CatalogMock : IMockHttpServerBuilder
    {
        private readonly Subjects subjects;
        public CatalogMock(string filename) => this.subjects = JsonConvert.DeserializeObject<Subjects>(File.ReadAllText(filename));
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
                            Name = $"Item {r.PathSegments[3]}",
                            Sku = r.PathSegments[3],
                            UnitPrice = new decimal(rnd.Next(10000) / 100.0)
                        }))
                );
            server
                .Given(
                    Request.Create().WithPath("/runtime/policy/Shopping*")
                        .UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody((Func<IRequestMessage, string>)(_ => JsonConvert.SerializeObject((object)this.subjects.SubjectsList.First().Policies[0].Authorization))));
                        //.WithBody(r => this.subjects.SubjectsList.First().ToJson())
                
        }
    }
}
