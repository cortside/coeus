using Microsoft.Extensions.Configuration;
using PolicyServer.Mocks;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Util;

namespace PolicyServer
{
    static class Program
    {
        private static int sleepTime = 30000;
        private static BaseWireMock server;

        static void Main(string[] args)
        {
            IConfigurationRoot configuration = SetupConfiguration();
            SetupLogger(configuration);
            Log.Logger.Debug("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

            server = new BaseWireMock(Guid.NewGuid().ToString())
                .ConfigureBuilder<CommonWireMock>()
                .ConfigureBuilder<IdsMock>()
                .ConfigureBuilder<SubjectMock>()
                .ConfigureBuilder<CatalogMock>();

            Log.Logger.Debug(server.mockServer.Urls.First());

            server.mockServer.Given(Request.Create().WithPath("/api/sap")
                .UsingPost()
                .WithBody((IBodyData xmlData) =>
                {
                    //xmlData is always null
                    return true;
                }))
                .RespondWith(Response.Create().WithStatusCode(System.Net.HttpStatusCode.OK));

            server.mockServer
                .Given(Request.Create()
                    .UsingAnyMethod())
                .RespondWith(Response.Create()
                    .WithTransformer()
                    .WithBody("{{Random Type=\"Integer\" Min=100 Max=999999}} {{DateTime.Now}} {{DateTime.Now \"yyyy-MMM\"}} {{String.Format (DateTime.Now) \"MMM-dd\"}}"));

            Console.WriteLine($"{DateTime.UtcNow} Press Ctrl+C to shut down");
            Console.CancelKeyPress += (s, e) =>
            {
                Stop("CancelKeyPress");
            };

            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += ctx =>
            {
                Stop("AssemblyLoadContext.Default.Unloading");
            };

            while (true)
            {
                Console.WriteLine($"{DateTime.UtcNow} WireMock.Net server running : {server.IsStarted}");
                Thread.Sleep(sleepTime);
            }
        }

        private static void SetupLogger(IConfigurationRoot configuration)
        {
            var loggerConfiguration = new LoggerConfiguration()
                      .ReadFrom.Configuration(configuration)
                      .Enrich.FromLogContext();

            var serverUrl = configuration["Seq:ServerUrl"];
            if (!string.IsNullOrWhiteSpace(serverUrl))
            {
                loggerConfiguration.WriteTo.Seq(serverUrl);
            }
            Log.Logger = loggerConfiguration.CreateLogger();
        }

        private static IConfigurationRoot SetupConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
                .AddJsonFile("build.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void Stop(string why)
        {
            Console.WriteLine($"{DateTime.UtcNow} WireMock.Net server stopping because '{why}'");
            server.mockServer.Stop();
            Console.WriteLine($"{DateTime.UtcNow} WireMock.Net server stopped");
        }
    }
}
