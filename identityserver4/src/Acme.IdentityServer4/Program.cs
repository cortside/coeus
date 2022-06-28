using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cortside.Bowdlerizer;
using Cortside.IdentityServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Bowdlerizer;

namespace Cortside.IdentityServer.WebApi {

    public static class Program {

        private static string Environment =>
            System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        /// <summary>
        /// Load logging configuration for the app
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.local.json", optional: true)
            .AddJsonFile("config.json", false, false)
            .AddJsonFile("config.local.json", true, true)
            .AddJsonFile("build.json", false, true)
            .AddEnvironmentVariables()
            .Build();

        /// <summary>
        /// Startup method
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(string[] args) {
            var config = Configuration;
            var build = Configuration.GetSection("Build").Get<Build>();

            var rules = Configuration.GetSection("Bowdlerizer").Get<List<BowdlerizerRuleConfiguration>>();
            var bowdlerizer = new Bowdlerizer(rules);

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", Environment)
                .Enrich.WithProperty("service", System.Reflection.Assembly.GetEntryAssembly().FullName)
                .Enrich.WithProperty("build-version", build.Version)
                .Enrich.WithProperty("build-tag", build.Tag)
                .Destructure.UsingBowdlerizer(bowdlerizer)
                .Enrich.WithBowdlerizer(bowdlerizer);

            var serverUrl = Configuration["Seq:ServerUrl"];
            if (!String.IsNullOrWhiteSpace(serverUrl)) {
                loggerConfiguration.WriteTo.Seq(serverUrl);
            }
            Log.Logger = loggerConfiguration.CreateLogger();
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            try {
                Log.Information("Starting web host");
                BuildWebHost(args, config, bowdlerizer).Build().Run();
                return 0;
            } catch (Exception ex) {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            } finally {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder BuildWebHost(string[] args, IConfiguration config, Bowdlerizer bowdlerizer) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => {
                builder.AddConfiguration(config);
            })
             .ConfigureWebHostDefaults(webBuilder => {
                 webBuilder.UseKestrel(options => options.AddServerHeader = false);
                 webBuilder.UseIISIntegration();
                 webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                 webBuilder.UseConfiguration(config);
                 webBuilder.UseStartup<Startup>();
                 webBuilder.UseSerilog();
             })
            .ConfigureServices(services => {
                services.AddSingleton(bowdlerizer);
            });
    }
}
