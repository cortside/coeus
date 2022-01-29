﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Acme.ShoppingCart.UserClient.Models.Responses;
using Acme.ShoppingCart.UserClient.Tests.Mock;
using Cortside.RestSharpClient.Authenticators.OpenIDConnect;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using RestSharp;
using RichardSzalay.MockHttp;
using Xunit;

namespace Acme.ShoppingCart.UserClient.Tests {
    public class UserClientTest {
        private readonly IUserClient userClient;
        private readonly UserClientConfiguration config;

        public UserClientTest() {
            UserWireMock userMock = new UserWireMock();
            var wiremockurl = userMock.fluentMockServer.Urls.First();
            var request = new TokenRequest {
                AuthorityUrl = wiremockurl,
                ClientId = "clientid",
                ClientSecret = "secret",
                GrantType = "client_credentials",
                Scope = "user-api",
                SlidingExpiration = 30
            };
            config = new UserClientConfiguration { ServiceUrl = wiremockurl, Authentication = request };
            userClient = new UserClient(config, new Logger<UserClient>(new NullLoggerFactory()));
        }

        [Fact]
        public async Task WireMock_ShouldGetUserAsync() {
            //arrange
            Guid userId = Guid.NewGuid();

            //act
            UserInfoResponse user = await userClient.GetUserByIdAsync(userId).ConfigureAwait(false);

            //assert
            user.Should().NotBeNull();
        }

        [Fact]
        public async Task MockHttpMessageHandler_ShouldGetUserAsync() {
            // arrange
            var user = new UserInfoResponse() {
                UserId = Guid.NewGuid(),
                FirstName = "first",
                LastName = "last",
                EmailAddress = "first@last.com"
            };
            var json = JsonConvert.SerializeObject(user);

            string url = "http://localhost:1234";
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{url}/api/v1/users/*")
                    .Respond("application/json", json);

            var options = new RestClientOptions {
                BaseUrl = new Uri(url),
                ConfigureMessageHandler = _ => mockHttp
            };
            var client = new UserClient(config, new Logger<UserClient>(new NullLoggerFactory()), options);

            //act
            UserInfoResponse response = await client.GetUserByIdAsync(user.UserId).ConfigureAwait(false);

            //assert
            response.Should().BeEquivalentTo(user);
        }
    }
}
