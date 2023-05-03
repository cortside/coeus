using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;
using Acme.IdentityServer.WebApi.Helpers;
using Acme.IdentityServer.WebApi.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Helpers {
    public class HtmlTemplateHelperTests {
        private HtmlTemplateHelper sut;

        public HtmlTemplateHelperTests() {
            var configMock = new Mock<IConfigurationRoot>();

            var fileSystemMock = new Mock<IFileSystem>();
            fileSystemMock.Setup(x => x.File.ReadAllText(It.IsAny<string>())).Returns(GetDefaultHtmlTemplate());

            var htmlLocationFinder = new Mock<IHtmlTemplateLocationFinder>();

            sut = new HtmlTemplateHelper(configMock.Object, fileSystemMock.Object, htmlLocationFinder.Object);
        }

        [Fact]
        public void ShouldGetHtmlTemplate() {
            // Arrange

            // Act
            var result = sut.GetHtmlTemplate(HtmlTemplateType.ClientSecretEmail);

            // Assert
            Assert.Contains("Test Html", result);
        }

        [Fact]
        public void ShouldGetHtmlTemplateWithTokenValues() {
            // Arrange
            var newTokenValue1 = "newTokenValue1";
            var newTokenValue2 = "newTokenValue2";

            var tokenValues = new List<HtmlTemplateParameter>() {
                new HtmlTemplateParameter() {
                    ParameterName = "{{token}}",
                    ParameterValue = newTokenValue1
                },
                new HtmlTemplateParameter() {
                    ParameterName = "{{token2}}",
                    ParameterValue = newTokenValue2
                }
            };

            // Act
            var result = sut.GetHtmlTemplate(HtmlTemplateType.ClientSecretEmail, tokenValues);

            // Assert
            Assert.Contains("Test Html", result);
            Assert.Contains(newTokenValue1, result);
            Assert.Contains(newTokenValue2, result);
        }

        private string GetDefaultHtmlTemplate() {
            var sb = new StringBuilder();
            sb.AppendLine("<head>");
            sb.AppendLine(" <title>Test Html</title>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine(" <div>{{token}}</div>");
            sb.AppendLine(" <div>{{token2}}</div>");
            sb.AppendLine("</body>");

            return sb.ToString();
        }
    }
}
