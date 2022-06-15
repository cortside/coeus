using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Microsoft.Extensions.Configuration;
using PolicyServer.Mocks;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace PolicyServer
{
    static class Program
    {
        private static int sleepTime = 30000;
        private static MockHttpServer server;

        static void Main(string[] args)
        {
            IConfigurationRoot configuration = SetupConfiguration();
            SetupLogger(configuration);
            Log.Logger.Debug("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

            server = new MockHttpServer(Guid.NewGuid().ToString(), 5001, Log.Logger)
                .ConfigureBuilder<CommonMock>()
                .ConfigureBuilder(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .ConfigureBuilder(new SubjectMock("./Data/subjects.json"))
                .ConfigureBuilder<CatalogMock>();

            Log.Logger.Debug($"Server is listening at {server.Url}");

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
            Console.WriteLine($"{DateTime.UtcNow} Server stopping because '{why}'");
            server.Stop();
            Console.WriteLine($"{DateTime.UtcNow} Server stopped");
        }
    }
}
