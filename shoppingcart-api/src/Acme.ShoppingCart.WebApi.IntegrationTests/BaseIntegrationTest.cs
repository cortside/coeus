namespace Acme.ShoppingCart.WebApi.IntegrationTests {
    public class BaseIntegrationTest {
        private readonly WebApiApplicationFactory api;

        public BaseIntegrationTest() {
            api = new WebApiApplicationFactory();
        }
    }
}
