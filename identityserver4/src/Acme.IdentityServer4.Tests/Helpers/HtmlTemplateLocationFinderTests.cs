using EnerBank.IdentityServer.WebApi.Helpers;
using EnerBank.IdentityServer.WebApi.Models;
using Xunit;

namespace EnerBank.IdentityServer.WebApi.Tests.Helpers {
    public class HtmlTemplateLocationFinderTests {
        private HtmlTemplateLocationFinder sut;

        public HtmlTemplateLocationFinderTests() {

            sut = new HtmlTemplateLocationFinder();
        }

        [Fact]
        public void ShouldReturnClientSecretEmail() {
            // Arrange
            var type = HtmlTemplateType.ClientSecretEmail;

            // Act
            var result = sut.GetLocation(type);

            // Assert
            Assert.Equal("ClientSecretEmailHtmlTemplate.html", result);
        }
    }
}
