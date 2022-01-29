using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Acme.ShoppingCart.WebApi.Controllers;
using Cortside.Health.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class DependencyInjectionTest : IClassFixture<IntegrationTestFactory<Startup>> {
        private readonly IntegrationTestFactory<Startup> fixture;
        private readonly ITestOutputHelper testOutputHelper;

        public DependencyInjectionTest(IntegrationTestFactory<Startup> fixture, ITestOutputHelper testOutputHelper) {
            this.fixture = fixture;
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void VerifyControllerResolution() {
            var controllersAssembly = typeof(HealthController).Assembly;
            var controllers = controllersAssembly.ExportedTypes.Where(x => typeof(ControllerBase).IsAssignableFrom(x) && !x.IsAbstract).ToList();

            controllersAssembly = typeof(AuthorizationController).Assembly;
            controllers.AddRange(controllersAssembly.ExportedTypes.Where(x => typeof(ControllerBase).IsAssignableFrom(x) && !x.IsAbstract));

            var activator = fixture.Services.GetService<IControllerActivator>();
            var serviceProvider = fixture.Services.GetService<IServiceProvider>();
            var errors = new Dictionary<Type, Exception>();

            var count = 0;
            var min = long.MaxValue;
            var max = long.MinValue;
            long total = 0;
            var slowest = string.Empty;

            foreach (var controllerType in controllers) {
                try {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var actionContext = new ActionContext(
                        new DefaultHttpContext {
                            RequestServices = serviceProvider
                        },
                        new RouteData(),
                        new ControllerActionDescriptor {
                            ControllerTypeInfo = controllerType.GetTypeInfo()
                        });
                    var controller = activator.Create(new ControllerContext(actionContext));
                    stopwatch.Stop();

                    if (stopwatch.ElapsedMilliseconds > max) {
                        max = stopwatch.ElapsedMilliseconds;
                        slowest = controller.GetType().ToString();
                    }
                    if (stopwatch.ElapsedMilliseconds < min) {
                        min = stopwatch.ElapsedMilliseconds;
                    }
                    count++;
                    total += stopwatch.ElapsedMilliseconds;

                    if (stopwatch.ElapsedMilliseconds > 100) {
                        Console.Out.WriteLine($"Resolved controller {controller.GetType()} in {stopwatch.ElapsedMilliseconds}ms");
                    }
                } catch (Exception e) {
                    Console.Out.WriteLine($"Failed to resolve controller {controllerType} due to {e}");
                    errors.Add(controllerType, e);
                }
            }

            Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors.Select(x => $"Failed to resolve controller {x.Key.Name} due to {x.Value}")));
        }
    }
}
