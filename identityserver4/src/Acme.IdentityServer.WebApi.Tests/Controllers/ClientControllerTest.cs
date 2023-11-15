using System.Net;
using Acme.IdentityServer.WebApi.Controllers.Client;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Controllers {

    public class ClientControllerTest : BaseTestFixture {

        ClientController clientsController;
        Mock<IClientService> clientsServiceMock;

        public ClientControllerTest() {
            clientsServiceMock = new Mock<IClientService>();
            clientsController = new ClientController(clientsServiceMock.Object);
        }

        [Fact]
        public void Update_ShouldReturnSuccessWithResponseBody_IfSuccessful() {
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client();

            clientsServiceMock
                .Setup(x => x.UpdateClient(It.IsAny<string>(), It.IsAny<UpdateClientRequest>()))
                .Returns(testClient);

            var result = clientsController.Update("test", new UpdateClientRequest() {
                GrantType = ClientConstants.GrantTypes.Implicit
            }) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(testClient, result.Value);
        }

        [Fact]
        public void Update_ShouldReturnBadRequest_IfGrantTypeIsUnrecognized() {

            SetupControllerWithRequest(new HeaderDictionary());

            var result = clientsController.Update("test", new UpdateClientRequest() {
                GrantType = "invalid"
            }) as BadRequestResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void Delete_ShouldReturnSuccess_IfSuccessful() {
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client();

            clientsServiceMock
                .Setup(x => x.DeleteClient(It.IsAny<string>()));

            var result = clientsController.Delete("test") as OkResult;

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

            clientsController.ControllerContext = new ControllerContext {
                HttpContext = httpContext.Object
            };
        }
    }
}
