using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Exceptions;
using Acme.ShoppingCart.UserClient.Models.Responses;
using Cortside.RestSharpClient;
using Cortside.RestSharpClient.Authenticators.OpenIDConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Acme.ShoppingCart.UserClient {
    public class UserClient : IDisposable, IUserClient {
        private readonly RestSharpClient client;
        private readonly ILogger<UserClient> logger;

        public UserClient(UserClientConfiguration userClientConfiguration, ILogger<UserClient> logger) {
            this.logger = logger;
            var options = new RestClientOptions {
                BaseUrl = new Uri(userClientConfiguration.ServiceUrl),
                FollowRedirects = true
            };
            client = new RestSharpClient(options, logger) {
                Authenticator = new OpenIDConnectAuthenticator(userClientConfiguration.Authentication),
                Serializer = new JsonNetSerializer(),
                Cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()))
            };
        }

        public UserClient(UserClientConfiguration userClientConfiguration, ILogger<UserClient> logger, RestClientOptions options) {
            this.logger = logger;
            client = new RestSharpClient(options, logger) {
                Authenticator = new OpenIDConnectAuthenticator(userClientConfiguration.Authentication),
                Serializer = new JsonNetSerializer(),
                Cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()))
            };
        }

        public async Task<UserInfoResponse> GetUserByIdAsync(Guid userId) {
            logger.LogInformation($"Getting User by ID: {userId}.");
            RestRequest request = new RestRequest($"api/v1/users/{userId}", Method.Get);
            try {
                var response = await client.GetAsync<UserInfoResponse>(request).ConfigureAwait(false);
                return response.Data;
            } catch (Exception ex) {
                logger.LogError($"Error contacting user api to retrieve user info for {userId}.");
                throw new ExternalCommunicationFailureMessage($"Error contacting user api to retrieve user info for {userId}.", ex);
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            client?.Dispose();
        }
    }
}
