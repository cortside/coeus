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
    public class CatalogClient : IDisposable, ICatalogClient {
        private readonly RestSharpClient client;
        private readonly ILogger<CatalogClient> logger;

        public CatalogClient(CatalogClientConfiguration userClientConfiguration, ILogger<CatalogClient> logger) {
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

        public CatalogClient(CatalogClientConfiguration userClientConfiguration, ILogger<CatalogClient> logger, RestClientOptions options) {
            this.logger = logger;
            client = new RestSharpClient(options, logger) {
                Authenticator = new OpenIDConnectAuthenticator(userClientConfiguration.Authentication),
                Serializer = new JsonNetSerializer(),
                Cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()))
            };
        }

        public async Task<CatalogItem> GetItem(string sku) {
            logger.LogInformation($"Getting item by sku: {sku}.");
            RestRequest request = new RestRequest($"api/v1/items/{sku}", Method.Get);
            try {
                var response = await client.GetAsync<CatalogItem>(request).ConfigureAwait(false);
                return response.Data;
            } catch (Exception ex) {
                logger.LogError($"Error contacting user api to retrieve user info for {sku}.");
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
