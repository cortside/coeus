using System;
using System.Globalization;
using System.IO;
using Acme.ShoppingCart.WebApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Acme.ShoppingCart.WebApi {
    /// <summary>
    /// Program
    /// </summary>
    public static class Program {
        /// <summary>
        /// Running environment
        /// </summary>
        public static string Environment => System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        /// <summary>
        /// Load logging configuration for the app
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
            .AddJsonFile("build.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(string[] args) {
            var config = Configuration;
            // TODO:  use version from health?
            var build = Configuration.GetSection("Build").Get<Build>();
            var service = Configuration["Service:Name"];

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", Environment)
                .Enrich.WithProperty("Service", service)
                .Enrich.WithProperty("BuildVersion", build.Version)
                .Enrich.WithProperty("BuildTag", build.Tag);

            var serverUrl = Configuration["Seq:ServerUrl"];
            if (!String.IsNullOrWhiteSpace(serverUrl)) {
                loggerConfiguration.WriteTo.Seq(serverUrl);
            }
            Log.Logger = loggerConfiguration.CreateLogger();

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            try {
                Log.Information("Starting {Service}");
                Log.Information("ASPNETCORE environment = {Environment}");

                var host = CreateHostBuilder(args, config).Build();

                host.Run();
                return 0;
            } catch (Exception ex) {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            } finally {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Create the host builder
        /// </summary>
        /// <param name="args"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => {
                    builder.AddConfiguration(configuration);
                })
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                    webBuilder.UseKestrel(k => {
                        k.Limits.MaxRequestLineSize = int.MaxValue;
                        k.Limits.MaxRequestBufferSize = int.MaxValue;
                    });
                });
    }
}
