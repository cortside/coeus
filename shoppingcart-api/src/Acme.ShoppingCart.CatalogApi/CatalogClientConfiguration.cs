using Cortside.RestApiClient.Authenticators.OpenIDConnect;

namespace Acme.ShoppingCart.CatalogApi {
    public class CatalogClientConfiguration {
        public string ServiceUrl { get; set; }
        public TokenRequest Authentication { get; set; }
    }
}
