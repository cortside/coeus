using System;
using System.Threading.Tasks;
using Acme.ShoppingCart.Exceptions;
using Acme.ShoppingCart.UserClient.Models.Responses;
using Cortside.RestApiClient;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Acme.ShoppingCart.UserClient {
    public class CatalogClient : IDisposable, ICatalogClient {
        private readonly RestApiClient client;
        private readonly ILogger<CatalogClient> logger;

        public CatalogClient(CatalogClientConfiguration userClientConfiguration, ILogger<CatalogClient> logger) {
            this.logger = logger;
            var options = new RestApiClientOptions {
                BaseUrl = new Uri(userClientConfiguration.ServiceUrl),
                FollowRedirects = true,
                Authenticator = new OpenIDConnectAuthenticator(userClientConfiguration.Authentication),
                Serializer = new JsonNetSerializer(),
                Cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()))
            };
            client = new RestApiClient(logger, options);
        }

        public CatalogClient(CatalogClientConfiguration userClientConfiguration, ILogger<CatalogClient> logger, RestApiClientOptions options) {
            this.logger = logger;
            client = new RestApiClient(logger, options);
        }

        public async Task<CatalogItem> GetItemAsync(string sku) {
            logger.LogInformation("Getting item by sku: {sku}", sku);
            RestApiRequest request = new RestApiRequest($"api/v1/items/{sku}", Method.Get);
            try {
                var response = await client.GetAsync<CatalogItem>(request).ConfigureAwait(false);
                return response.Data;
            } catch (Exception ex) {
                logger.LogError("Error contacting user api to retrieve user info for {sku}", sku);
                throw new ExternalCommunicationFailureMessage($"Error contacting user api to retrieve user info for {sku}.", ex);
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
