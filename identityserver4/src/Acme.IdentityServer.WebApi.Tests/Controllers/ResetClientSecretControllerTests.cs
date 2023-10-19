using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Controllers.ResetClientSecretController;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Models.Output;
using Acme.IdentityServer.WebApi.Services;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Controllers {
    public class ResetClientSecretControllerTests : BaseTestFixture {
        private readonly ResetClientSecretController sut;
        private readonly Mock<IClientSecretService> clientSecretServiceMock;
        private readonly Mock<IResetClientSecretService> resetClientSecretServiceMock;
        private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
        HttpContext httpContext = null;

        public ResetClientSecretControllerTests() {
            clientSecretServiceMock = new Mock<IClientSecretService>();
            clientSecretServiceMock.Setup(x => x.SendVerificationCode(It.IsAny<Guid>(), It.IsAny<SendVerificationCodeModel>())).Returns(Task.CompletedTask);
            var isVerificationCodeOutput = new IsVerificationCodeValidOutput() {
                IsValid = true
            };

            clientSecretServiceMock.Setup(x => x.IsVerificationCodeValid(It.IsAny<Guid>(), It.IsAny<string>())).Returns(isVerificationCodeOutput);

            resetClientSecretServiceMock = new Mock<IResetClientSecretService>();
            var verifyIdentityModel = new VerifyIdentityModel() {
                Last4PhoneNumber = "1234",
                RequestId = Guid.NewGuid(),
                TokenHash = "testhash",
                VerificationCode = "123456"
            };

            resetClientSecretServiceMock.Setup(x => x.BuildVerifyIdentityViewModel(It.IsAny<Guid>())).Returns(verifyIdentityModel);

            httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            httpContext = new DefaultHttpContext() {
                RequestServices = CreateServices()
            };

            httpContextAccessorMock.Setup(c => c.HttpContext).Returns(httpContext);

            sut = new ResetClientSecretController(clientSecretServiceMock.Object, resetClientSecretServiceMock.Object, httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task Index_ShouldReturnInvalidLinkView() {
            // Arrange

            // Act
            var result = await sut.Index() as ViewResult;

            // Assert
            Assert.Equal("InvalidLink", result.ViewName);
        }

        [Fact]
        public void VerifyIdentity_ShouldErrorWithoutVerificationCode() {
            // Arrange
            var requestId = Guid.NewGuid().ToString();
            var tokenHash = "testhash";
            var button = "submit";
            var errorMessage = "Verification Code is required.";

            // Act
            sut.VerifyIdentity(requestId, null, tokenHash, button);
            sut.ModelState.TryGetValue("submiterror", out var errorValue);

            // Assert
            Assert.Equal(errorMessage, errorValue.Errors.FirstOrDefault(x => x.ErrorMessage == errorMessage).ErrorMessage);
        }

        [Fact]
        public void VerifyIdentity_ShouldReturnSecretKeyView() {
            // Arrange
            var requestId = Guid.NewGuid().ToString();
            var verificationCode = "123456";
            var tokenHash = "testhash";
            var button = "submit";

            // Act
            var result = sut.VerifyIdentity(requestId, verificationCode, tokenHash, button) as ViewResult;

            // Assert
            Assert.Equal("SecretKey", result.ViewName);
        }

        [Fact]
        public async Task ResendCode_ShouldReturnNoContent() {
            // Arrange
            var requestId = Guid.NewGuid().ToString();
            var tokenHash = "testhash";

            // Act
            var result = await sut.ResendCode(requestId, tokenHash);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void SecretKey_ShouldReturnSecretKey() {
            // Arrange
            var requestId = Guid.NewGuid().ToString();

            // Act
            var result = sut.SecretKey(requestId) as ViewResult;

            // Assert
            Assert.Equal("SecretKey", result.ViewName);
        }

        /// <summary>
        /// Create IServiceProvider that registers services required in order to create HttpContext
        /// </summary>
        /// <returns></returns>
        private IServiceProvider CreateServices() {
            var serviceProviderMock = new Mock<IServiceProvider>();

            var idsOptions = new IdentityServerOptions();
            //idsOptions.Authentication = "Cookies";
            serviceProviderMock.Setup(p => p.GetService(typeof(IdentityServerOptions))).Returns(idsOptions);

            var dicMock = new Mock<ITempDataDictionaryFactory>();
            serviceProviderMock.Setup(p => p.GetService(typeof(ITempDataDictionaryFactory))).Returns(dicMock.Object);


            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<Microsoft.AspNetCore.Authentication.AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));
            serviceProviderMock.Setup(_ => _.GetService(typeof(IAuthenticationService))).Returns(authServiceMock.Object);


            return serviceProviderMock.Object;
        }
    }
}
