using WireMock.Server;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Helpers.Mocks {
    public interface IWireMockBuilder {
        public void Configure(WireMockServer server);
    }
}
