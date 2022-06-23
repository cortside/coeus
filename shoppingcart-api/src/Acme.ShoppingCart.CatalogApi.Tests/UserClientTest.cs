using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.UserClient.Models.Responses;
using Acme.ShoppingCart.UserClient.Tests.Mock;
using Cortside.RestApiClient;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Xunit;

namespace Acme.ShoppingCart.UserClient.Tests {
    public class UserClientTest {
        private readonly ICatalogClient userClient;
        private readonly CatalogClientConfiguration config;

        public UserClientTest() {
            UserWireMock userMock = new UserWireMock();
            var wiremockurl = userMock.server.Urls[0];
            var request = new TokenRequest {
                AuthorityUrl = wiremockurl,
                ClientId = "clientid",
                ClientSecret = "secret",
                GrantType = "client_credentials",
                Scope = "user-api",
                SlidingExpiration = 30
            };
            config = new CatalogClientConfiguration { ServiceUrl = wiremockurl, Authentication = request };
            userClient = new CatalogClient(config, new Logger<CatalogClient>(new NullLoggerFactory()));
        }

        [Fact]
        public async Task WireMock_ShouldGetUserAsync() {
            //arrange
            string sku = Guid.NewGuid().ToString();

            //act
            CatalogItem item = await userClient.GetItemAsync(sku).ConfigureAwait(false);

            //assert
            item.Should().NotBeNull();
        }

        [Fact]
        public async Task MockHttpMessageHandler_ShouldGetUserAsync() {
            // arrange
            var user = new CatalogItem() {
                ItemId = Guid.NewGuid(),
                Name = "Foo",
                Sku = Guid.NewGuid().ToString(),
                UnitPrice = 12.34M
            };
            var json = JsonConvert.SerializeObject(user);

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
            var client = new CatalogClient(config, new Logger<CatalogClient>(new NullLoggerFactory()), options);

            //act
            CatalogItem response = await client.GetItemAsync(user.Sku).ConfigureAwait(false);

            //assert
            response.Should().BeEquivalentTo(user);
        }
    }
}
