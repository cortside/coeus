using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PolicyServer.Mocks
{
    public class CommonWireMock : IWireMockBuilder
    {
        public void Configure(WireMockServer server)
        {
            server.AddCatchAllMapping();
            server
                .Given(
                    Request.Create().WithPath("/".Split('?')[0]).UsingGet()
                    )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                    );

            server
                .Given(
                    Request.Create().WithPath("/").UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                );

            server
                .Given(
                    Request.Create().WithPath("/api/health").UsingGet()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                );
        }
    }
}
