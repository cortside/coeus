using System.Net.Http.Headers;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Helpers.Auth {
    public class Authentication {
        public AuthenticationHeaderValue GetBearerToken() {
            return new AuthenticationHeaderValue("Bearer", "107d225e3ea706bbbceb83593daf5c73c1347a89eaa89f253ca845beffb2e3f9");
        }
    }
}
