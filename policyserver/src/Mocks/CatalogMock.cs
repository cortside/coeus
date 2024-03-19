using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cortside.AspNetCore.Common.Paging;
using Cortside.MockServer;
using Cortside.MockServer.Builder;
using Newtonsoft.Json;
using PolicyServer.Mocks.Models;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace PolicyServer.Mocks {
    public class CatalogMock : IMockHttpMock {
        private readonly PagedList<CatalogItem> items = new PagedList<CatalogItem>();

        public CatalogMock(string filename) {
            items.Items = JsonConvert.DeserializeObject<List<CatalogItem>>(File.ReadAllText(filename));
            items.PageNumber = 1;
            items.PageSize = items.Items.Count;
            items.TotalItems = items.Items.Count;
        }

        public void Configure(MockHttpServer server) {
            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/api/v1/items")
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_ => JsonConvert.SerializeObject(items))
                );

            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/api/v1/items/*")
                        .WithPath((path) => GetItem(path.Trim('/').Split("/")[3]) == null)
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithNotFound()
                );

            server.WireMockServer
                .Given(
                    Request.Create().WithPath(new RegexMatcher(@"^\/api\/v1\/items\/[\w-]+$"))
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(r => JsonConvert.SerializeObject(GetItem(r.PathSegments[3])))
                );

            server.WireMockServer
                .Given(
                    Request.Create().WithPath(new RegexMatcher(@"^\/api\/v1\/items\/[\w-]+\/related$"))
                        .UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(r => JsonConvert.SerializeObject(GetRelatedItems(r.PathSegments[3])))
                );
        }

        private CatalogItem GetItem(string sku) {
            var item = items.Items.FirstOrDefault(x => x.Sku == sku);
            return item;
        }

        private PagedList<CatalogItem> GetRelatedItems(string sku) {
            var page = new PagedList<CatalogItem>();
            page.Items = new List<CatalogItem>(items.Items.Where(x => x.Sku != sku));
            page.PageSize = page.Items.Count;
            page.TotalItems = page.Items.Count;
            page.PageNumber = 1;
            return page;
        }
    }
}
