using System;
using System.Net;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Controllers.ClientSecretRequests;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Controllers {
    public class ClientSecretRequestsControllerTest : BaseTestFixture {
        private readonly ClientSecretRequestsController sut;
        private readonly Mock<IClientSecretService> clientSecretServiceMock;

        public ClientSecretRequestsControllerTest() {
            clientSecretServiceMock = new Mock<IClientSecretService>();

            sut = new ClientSecretRequestsController(clientSecretServiceMock.Object);
        }

        [Fact]
        public async Task ShoulSendVerificationCode() {
            // Arrange
            SetupControllerWithRequest(new HeaderDictionary());

            clientSecretServiceMock
                .Setup(x => x.SendVerificationCode(It.IsAny<Guid>(), It.IsAny<SendVerificationCodeModel>()))
                .Returns(Task.CompletedTask);

            var clientSecretRequestId = Guid.NewGuid();
            var model = new SendVerificationCodeModel() {
                TokenHash = "testhash"
            };

            // Act
            var result = await sut.SendVerificationCode(clientSecretRequestId, model) as OkResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        private void SetupControllerWithRequest(IHeaderDictionary requestHeaders) {
            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var response = new Mock<HttpResponse>();

            request.SetupGet(x => x.Headers).Returns(requestHeaders);
            response.SetupGet(x => x.Headers).Returns(new HeaderDictionary());
            httpContext.SetupGet(x => x.Request).Returns(request.Object);
            httpContext.SetupGet(x => x.Response).Returns(response.Object);

            sut.ControllerContext = new ControllerContext {
                HttpContext = httpContext.Object
            };
        }
    }
}
