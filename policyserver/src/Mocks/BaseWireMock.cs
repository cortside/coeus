using Serilog;
using System.Linq;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;

namespace PolicyServer.Mocks
{
    public class BaseWireMock
    {
        public WireMockServer mockServer;

        public BaseWireMock()
        {
            if (mockServer == null)
            {
                mockServer = WireMockServer.Start(new WireMockServerSettings
                {
                    CorsPolicyOptions = CorsPolicyOptions.AllowAll,
                    StartAdminInterface = true,
                    AllowCSharpCodeMatcher = true,
                    Logger = new WireMockLogger(Log.Logger),
                    Port = 5001
                });
            }
        }

        public BaseWireMock(string routePrefix) : this()
        {
            RoutePrefix = routePrefix;
        }

        public string RoutePrefix { get; }
        public string WireMockUrl => mockServer.Urls.First();
        public string WireMockRouteUrl => $"{WireMockUrl}/{RoutePrefix}";

        public bool IsStarted { get; internal set; }

        public BaseWireMock ConfigureBuilder<T>() where T : IWireMockBuilder, new()
        {
            new T().Configure(mockServer);
            return this;
        }

        internal void WaitForStart()
        {
            while (!mockServer.IsStarted)
            {
                // nothing to do here
            }
            IsStarted = true;
        }
    }
}
