using System;
using System.IO;
using System.Linq;
using System.Threading;
using Cortside.Common.Json;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PolicyServer.Mocks;
using Serilog;

namespace PolicyServer {
    static class Program {
        private const int sleepTime = 30000;
        private static MockHttpServer server;

        static void Main(string[] args) {
            IConfiguration configuration = SetupConfiguration();
            SetupLogger(configuration);
            Log.Logger.Debug("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

            JsonConvert.DefaultSettings = () => {
                var settings = new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                    DefaultValueHandling = DefaultValueHandling.Include,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    DateParseHandling = DateParseHandling.DateTimeOffset,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                settings.Converters.Add(new IsoTimeSpanConverter());
                settings.Converters.Add(new IsoDateTimeConverter {
                    DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"
                });

                return settings;
            };

            server = new MockHttpServer(Guid.NewGuid().ToString(), 5001, Log.Logger)
                .ConfigureBuilder<CommonMock>()
                .ConfigureBuilder(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .ConfigureBuilder(new SubjectMock("./Data/subjects.json"))
                .ConfigureBuilder(new CatalogMock("./Data/items.json"));

            Log.Logger.Debug("Server is listening at {Url}", server.Url);

            Console.WriteLine($"{DateTime.UtcNow} Press Ctrl+C to shut down");
            Console.CancelKeyPress += (s, e) => {
                Stop("CancelKeyPress");
            };

            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += ctx => {
                Stop("AssemblyLoadContext.Default.Unloading");
            };

            while (true) {
                Console.WriteLine($"{DateTime.UtcNow} WireMock.Net server running : {server.IsStarted}");
                Thread.Sleep(sleepTime);
            }
        }

        private static void SetupLogger(IConfiguration configuration) {
            var loggerConfiguration = new LoggerConfiguration()
                      .ReadFrom.Configuration(configuration)
                      .Enrich.FromLogContext();

            var serverUrl = configuration["Seq:ServerUrl"];
            if (!string.IsNullOrWhiteSpace(serverUrl)) {
                loggerConfiguration.WriteTo.Seq(serverUrl);
            }
            Log.Logger = loggerConfiguration.CreateLogger();
        }

        private static IConfiguration SetupConfiguration() {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
                .AddJsonFile("build.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void Stop(string why) {
            Console.WriteLine($"{DateTime.UtcNow} Server stopping because '{why}'");
            server.Stop();
            Console.WriteLine($"{DateTime.UtcNow} Server stopped");
        }
    }
}
