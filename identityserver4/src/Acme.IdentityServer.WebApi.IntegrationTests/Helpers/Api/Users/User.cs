using System.Net;
using FluentAssertions;
using RestSharp;
using RestSharp.Authenticators;

namespace Acme.IdentityServer.WebApi.IntegrationTests.Helpers.Api.Users {
    public class User {
        private IRestClient restClient;

        public User() {
            restClient = new RestClient("http://localhost:8000");
        }

        public IRestResponse Get(string userId, JwtAuthenticator authenticator, IRestClient restClient = null, HttpStatusCode expectedResponseCode = HttpStatusCode.OK) {
            if (restClient != null) {
                this.restClient = restClient;
            }
            IRestRequest getUserRequest = new RestRequest($"/api/Users/{userId}", Method.GET);
            restClient.Authenticator = authenticator;
            IRestResponse getUserResponse = restClient.Execute(getUserRequest);
            getUserResponse.StatusCode.Should().Be(expectedResponseCode, getUserResponse.Content);
            return getUserResponse;
        }
    }
}
