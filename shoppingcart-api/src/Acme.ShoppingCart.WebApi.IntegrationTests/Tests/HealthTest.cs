using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cortside.Health.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class HealthTest : IClassFixture<IntegrationTestFactory<Startup>> {
        private readonly IntegrationTestFactory<Startup> fixture;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly HttpClient testServerClient;

        public HealthTest(IntegrationTestFactory<Startup> fixture, ITestOutputHelper testOutputHelper) {
            this.fixture = fixture;
            this.testOutputHelper = testOutputHelper;
            testServerClient = fixture.CreateClient(new WebApplicationFactoryClientOptions {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task TestAsync() {
            //arrange

            //act          
            var success = false;
            HttpResponseMessage response = null;
            var timer = new Stopwatch();
            timer.Start();
            while (!success && timer.ElapsedMilliseconds < 30000) {
                await Task.Delay(500).ConfigureAwait(false);
                response = await testServerClient.GetAsync("api/health").ConfigureAwait(false);
                success = response.IsSuccessStatusCode;
            }

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var respObj = JsonConvert.DeserializeObject<HealthModel>(content);
            Assert.True(respObj.Healthy, content);
        }
    }
}
