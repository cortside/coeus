using System;
using System.Collections.Generic;
using System.Net;
using Acme.IdentityServer.WebApi.Controllers.Client;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Controllers {
    public class ClientV1ControllerTest : BaseTestFixture {

        ClientV1Controller _clientsController;
        Mock<IClientService> _clientsServiceMock;
        Mock<IClientSecretService> _clientSecretServiceMock;

        public ClientV1ControllerTest() {
            _clientsServiceMock = new Mock<IClientService>();
            _clientSecretServiceMock = new Mock<IClientSecretService>();
            _clientsController = new ClientV1Controller(_clientsServiceMock.Object, _clientSecretServiceMock.Object);
        }

        [Fact]
        public void ShouldCreateClient() {
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client();

            _clientsServiceMock
                .Setup(x => x.CreateClient(It.IsAny<CreateClientModel>()))
                .Returns(testClient);

            var result = _clientsController.Create(new CreateClientModel() {
                ClientId = "clientId",
                SubjectId = "subjectId",
                Email = "email",
                PhoneNumber = "phone"
            }) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(testClient, result.Value);
        }

        [Fact]
        public void ShouldNotClient_MissingClientId() {
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client();

            _clientsServiceMock
                .Setup(x => x.CreateClient(It.IsAny<CreateClientModel>()))
                .Returns(testClient);

            var result = _clientsController.Create(new CreateClientModel() {
                SubjectId = "subjectId"
            }) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("ClientId", result.Value.ToString());
        }

        [Fact]
        public void ShouldNotClient_MissingSubjectId() {
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client();

            _clientsServiceMock
                .Setup(x => x.CreateClient(It.IsAny<CreateClientModel>()))
                .Returns(testClient);

            var result = _clientsController.Create(new CreateClientModel() {
                ClientId = "clientId"
            }) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("SubjectId", result.Value.ToString());
        }

        [Fact]
        public void ShouldResetClientSecret() {
            // Arrange
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client {
                ClientSecret = new ClientSecret {
                    Value = "invalid-hash="
                }
            };

            _clientSecretServiceMock
                .Setup(x => x.ResetSecret(It.IsAny<int>()))
                .ReturnsAsync(testClient);

            var clientId = 1;

            // Act
            var result = _clientsController.ResetSecret(clientId) as OkResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void ShouldThrowWhenResetWithNullClientId() {
            SetupControllerWithRequest(new HeaderDictionary());

            var result = _clientsController.ResetSecret(0) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("Id cannot be null or empty", result.Value.ToString());
        }

        [Fact]
        public void ShouldUpdateClient() {
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client();

            _clientsServiceMock
                .Setup(x => x.UpdateClient(1, It.IsAny<UpdateClientModel>()))
                .Returns(testClient);

            var request = new UpdateClientModel();
            var result = _clientsController.Update(1, request) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void ShouldGetClientById() {
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client();

            _clientsServiceMock
                .Setup(x => x.GetClient(It.IsAny<int>()))
                .Returns(testClient);

            var result = _clientsController.Get(10) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(testClient, result.Value);
        }

        [Fact]
        public void ShouldGetClientBySubjectId() {
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client();

            _clientsServiceMock
                .Setup(x => x.GetClient(It.IsAny<string>()))
                .Returns(testClient);

            var result = _clientsController.GetClientBySubjectId(Guid.NewGuid().ToString()) as OkObjectResult;

            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(testClient, result.Value);
        }

        [Fact]
        public void ShouldGetClientBySubjectId_Empty() {
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client();

            _clientsServiceMock
                .Setup(x => x.GetClient(It.IsAny<string>()))
                .Returns(testClient);

            var result = _clientsController.GetClientBySubjectId(null) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void ShouldGetClientById_BadRequest() {
            SetupControllerWithRequest(new HeaderDictionary());

            var testClient = new Client();

            _clientsServiceMock
                .Setup(x => x.GetClient(It.IsAny<int>()))
                .Returns(testClient);

            var result = _clientsController.Get(0) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public void ShouldUpdateClientScopes() {
            // Arrange
            SetupControllerWithRequest(new HeaderDictionary());

            var clientId = 1;

            var model = GetDefaultClientScopes();

            _clientsServiceMock
                .Setup(x => x.UpdateClientScopes(It.IsAny<int>(), It.IsAny<UpdateClientScopesModel>()))
                .Returns(model);

            // Act
            var result = _clientsController.UpdateScopes(clientId, model) as OkObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void ShouldUpdateClientClaims() {
            // Arrange
            SetupControllerWithRequest(new HeaderDictionary());

            var clientId = 1;

            var model = GetDefaultClientClaims();

            _clientsServiceMock
                .Setup(x => x.UpdateClientClaims(It.IsAny<int>(), It.IsAny<UpdateClientClaimsModel>()))
                .Returns(model);

            // Act
            var result = _clientsController.UpdateClaims(clientId, model) as OkObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void ShouldThrowWhenUpdateScopesWithZeroClientId() {
            // Arrange
            SetupControllerWithRequest(new HeaderDictionary());

            var model = GetDefaultClientScopes();

            // Act
            var result = _clientsController.UpdateScopes(0, model) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("Id cannot be null or empty", result.Value.ToString());
        }

        private UpdateClientScopesModel GetDefaultClientScopes() {
            return new UpdateClientScopesModel() {
                Scopes = new List<string> {
                    "newScope"
                }
            };
        }

        private UpdateClientClaimsModel GetDefaultClientClaims() {
            return new UpdateClientClaimsModel() {
                Claims = new List<Models.UserClaimModel>() {
                    new Models.UserClaimModel() {
                        Type = "role",
                        Value = "application-api"
                    }
                }
            };
        }

        private void SetupControllerWithRequest(IHeaderDictionary requestHeaders) {
            var httpContext = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var response = new Mock<HttpResponse>();

            request.SetupGet(x => x.Headers).Returns(requestHeaders);
            response.SetupGet(x => x.Headers).Returns(new HeaderDictionary());
            httpContext.SetupGet(x => x.Request).Returns(request.Object);
            httpContext.SetupGet(x => x.Response).Returns(response.Object);

            _clientsController.ControllerContext = new ControllerContext {
                HttpContext = httpContext.Object
            };
        }
    }
}
