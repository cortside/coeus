using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.CatalogApi.Models.Responses;
using Acme.ShoppingCart.CatalogApi.Tests.Mock;
using Cortside.MockServer;
using Cortside.RestApiClient;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Xunit;

namespace Acme.ShoppingCart.CatalogApi.Tests {
    public class CatalogClientTest {
        private readonly ICatalogClient catalogClient;
        private readonly CatalogClientConfiguration config;

        public CatalogClientTest() {
            var server = MockHttpServer.CreateBuilder(Guid.NewGuid().ToString())
                .AddMock<CatalogMock>()
                .Build();

            var wiremockurl = server.Url;
            var request = new TokenRequest {
                AuthorityUrl = wiremockurl,
                ClientId = "clientid",
                ClientSecret = "secret",
                GrantType = "client_credentials",
                Scope = "user-api",
                SlidingExpiration = 30
            };
            config = new CatalogClientConfiguration { ServiceUrl = wiremockurl, Authentication = request };
            catalogClient = new CatalogClient(config, new Logger<CatalogClient>(new NullLoggerFactory()), new HttpContextAccessor());
        }

        [Fact]
        public async Task WireMock_ShouldGetUserAsync() {
            //arrange
            string sku = Guid.NewGuid().ToString();

            //act
            CatalogItem item = await catalogClient.GetItemAsync(sku).ConfigureAwait(false);

            //assert
            item.Should().NotBeNull();
        }

        [Fact]
        public async Task MockHttpMessageHandler_ShouldGetUserAsync() {
            // arrange
            var item = new CatalogItem() {
                ItemId = Guid.NewGuid(),
                Name = "Foo",
                Sku = Guid.NewGuid().ToString(),
                UnitPrice = 12.34M
            };
            var json = JsonConvert.SerializeObject(item);

            const string url = "http://localhost:1234";
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{url}/api/v1/items/*")
                    .Respond("application/json", json);

            var options = new RestApiClientOptions {
                BaseUrl = new Uri(url),
                ConfigureMessageHandler = _ => mockHttp,
                //Authenticator = new OpenIDConnectAuthenticator(userClientConfiguration.Authentication),
                Serializer = new JsonNetSerializer(),
                Cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()))
            };
            var client = new CatalogClient(config, new NullLogger<CatalogClient>(), new HttpContextAccessor(), options);

            //act
            CatalogItem response = await client.GetItemAsync(item.Sku).ConfigureAwait(false);

            //assert
            response.Should().BeEquivalentTo(item);
        }
    }
}
