using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Acme.ShoppingCart.DomainService.Tests {
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
    }
}
