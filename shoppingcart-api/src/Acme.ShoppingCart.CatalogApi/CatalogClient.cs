using System;
using System.Net;
using System.Threading.Tasks;
using Acme.ShoppingCart.CatalogApi.Models.Responses;
using Cortside.RestApiClient;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RestSharp;

namespace Acme.ShoppingCart.CatalogApi {
    public class CatalogClient : IDisposable, ICatalogClient {
        private readonly RestApiClient client;
        private readonly ILogger<CatalogClient> logger;

        public CatalogClient(CatalogClientConfiguration catalogClientConfiguration, ILogger<CatalogClient> logger, IHttpContextAccessor context, RestApiClientOptions options = null) {
            this.logger = logger;
            options ??= new RestApiClientOptions {
                BaseUrl = new Uri(catalogClientConfiguration.ServiceUrl),
                FollowRedirects = true,
                Authenticator = new OpenIDConnectAuthenticator(context, catalogClientConfiguration.Authentication)
                    .UsePolicy(PolicyBuilderExtensions.Handle<Exception>()
                        .OrResult(r => r.StatusCode == HttpStatusCode.Unauthorized || r.StatusCode == 0)
                        .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 2))
                    )
                    .UseLogger(logger),
                Serializer = new JsonNetSerializer(),
                Cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()))
            };
            client = new RestApiClient(logger, context, options);
        }

        public async Task<CatalogItem> GetItemAsync(string sku) {
            logger.LogInformation("Getting item by sku: {Sku}", sku);
            RestApiRequest request = new RestApiRequest($"api/v1/items/{sku}", Method.Get) {
                Policy = PolicyBuilderExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutException>()
                    .OrResult(x => x.StatusCode == 0 || x.StatusCode == System.Net.HttpStatusCode.Unauthorized || x.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    .WaitAndRetryAsync(PolicyBuilderExtensions.Jitter(1, 3))
            };

            var response = await client.GetAsync<CatalogItem>(request).ConfigureAwait(false);
            if (!response.IsSuccessful) {
                throw response.LoggedFailureException(logger, "Error contacting catalog api to retrieve item info for {0}", sku);
            }

            return response.Data;
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
