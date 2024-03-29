using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cortside.Health.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Tests {
    public class HealthTest : IClassFixture<WebApiApplicationFactory> {
        private readonly WebApiApplicationFactory webApi;
        private readonly HttpClient testServerClient;

        public HealthTest(WebApiApplicationFactory webApi) {
            this.webApi = webApi;
            testServerClient = this.webApi.CreateClient(new WebApplicationFactoryClientOptions {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task TestAsync() {
            //arrange
            var success = false;
            HttpResponseMessage response = null;
            var timer = new Stopwatch();
            timer.Start();

            //act          
            while (!success && timer.ElapsedMilliseconds < 45000) {
                await Task.Delay(500);
                response = await testServerClient.GetAsync("api/health");
                success = response.IsSuccessStatusCode;
            }

            //assert
            var content = await response.Content.ReadAsStringAsync();
            var respObj = JsonConvert.DeserializeObject<HealthModel>(content, webApi.SerializerSettings);
            Assert.True(respObj.Healthy, content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
