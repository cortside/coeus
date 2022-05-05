using WireMock.Server;

namespace PolicyServer.Mocks
{
    public interface IWireMockBuilder
    {
        public void Configure(WireMockServer server);
    }
}
