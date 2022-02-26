using Cortside.RestSharpClient.Authenticators.OpenIDConnect;

namespace Acme.ShoppingCart.UserClient {
    public class CatalogClientConfiguration {
        public string ServiceUrl { get; set; }
        public TokenRequest Authentication { get; set; }
    }
}
