using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cortside.AspNetCore;
using Cortside.DomainEvent;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Acme.ShoppingCart.WebApi.Tests {
    public class HttpHelperTest {
        private readonly Mock<HttpRequest> httpRequestMock;

        public HttpHelperTest() {
            httpRequestMock = new Mock<HttpRequest>();
        }

        [Fact]
        public void ShouldReturnUrlFromRequest() {
            // arrange
            httpRequestMock.SetupGet(h => h.Path).Returns(new PathString("/stuff"));
            httpRequestMock.SetupGet(h => h.Host).Returns(new HostString("localhost"));
            httpRequestMock.SetupGet(h => h.PathBase).Returns(new PathString("/big"));
            httpRequestMock.SetupGet(h => h.Scheme).Returns("http");

            // act
            var url = HttpHelper.BuildUriFromRequest(httpRequestMock.Object);

            // assert
            url.Should().Be("http://localhost/big/stuff");
        }

        [Fact]
        public void ShouldReturnUrlFromRequestWithProtoHeader() {
            // arrange
            httpRequestMock.SetupGet(h => h.Path).Returns(new PathString("/stuff"));
            httpRequestMock.SetupGet(h => h.Host).Returns(new HostString("localhost"));
            httpRequestMock.SetupGet(h => h.PathBase).Returns(new PathString("/big"));
            httpRequestMock.SetupGet(h => h.Scheme).Returns("https");
            httpRequestMock.SetupGet(h => h.Headers).Returns(new HeaderDictionary { { "x-forwarded-proto", "http" } });

            // act
            var url = HttpHelper.BuildUriFromRequest(httpRequestMock.Object);

            // assert
            url.Should().Be("http://localhost/big/stuff");
        }

        [Theory]
        [InlineData("ServiceBus:Topic")]
        [InlineData("ServiceBus:Exchange")]
        public async Task ShouldParseTopic(string key) {
            // arrange
            var value = Guid.NewGuid().ToString();
            var dictionary = new Dictionary<string, string> {
                {key, value},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(dictionary)
                .Build();

            // act
            var settings = configuration.GetSection("ServiceBus").Get<DomainEventPublisherSettings>();

            // assert
            Assert.Equal(value, settings.Topic);
        }
    }
}
