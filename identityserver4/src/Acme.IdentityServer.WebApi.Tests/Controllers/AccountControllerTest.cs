using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Controllers.Account;
using Acme.IdentityServer.WebApi.Services;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Controllers {

    public class AccountControllerTest : BaseTestFixture {
        private readonly AccountController accountController;
        private readonly Mock<IAuthenticator> authenticatorMock;
        private readonly Mock<IAccountService> accountServiceMock;
        private readonly Mock<IIdentityServerInteractionService> idsServiceMock;
        private readonly Mock<IPersistedGrantService> persistedGrantServiceMock;
        private readonly Mock<IConfiguration> configMock;
        private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
        private readonly Mock<IEventService> eventsService;
        private readonly Mock<IUserService> userServiceMock;
        private readonly HttpContext httpContext;

        private readonly ActionDescriptor actionDescriptor;
        private readonly RouteData routeData;

        public AccountControllerTest() {
            authenticatorMock = new Mock<IAuthenticator>();
            accountServiceMock = new Mock<IAccountService>();
            idsServiceMock = new Mock<IIdentityServerInteractionService>();
            persistedGrantServiceMock = new Mock<IPersistedGrantService>();
            eventsService = new Mock<IEventService>();
            httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            configMock = new Mock<IConfiguration>();
            userServiceMock = new Mock<IUserService>();

            httpContext = new DefaultHttpContext() {
                RequestServices = CreateServices()
            };

            httpContextAccessorMock.Setup(c => c.HttpContext).Returns(httpContext);

            actionDescriptor = new ControllerActionDescriptor() { /* ??? */ };
            routeData = new RouteData();
            accountController = new AccountController(
                new NullLogger<AccountController>(),
                authenticatorMock.Object,
                configMock.Object,
                httpContextAccessorMock.Object,
                accountServiceMock.Object,
                eventsService.Object,
                idsServiceMock.Object,
                persistedGrantServiceMock.Object,
                userServiceMock.Object, new List<Provider>()
            ) {
                ControllerContext = new ControllerContext(new ActionContext(httpContext, routeData, actionDescriptor))
            };
        }

        [Fact]
        public async Task ShouldRemoveSubjectGrantsUponLogout() {
            // arrange
            // setup logged in user
            string userSub = Guid.NewGuid().ToString();
            var claims = new List<Claim> { new Claim(JwtClaimTypes.Subject, userSub) };
            var identity = new ClaimsIdentity(claims, "Custom");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            httpContext.User = claimsPrincipal;

            var model = new LogoutInputModel() {
                LogoutId = Guid.NewGuid().ToString()
            };
            var logoutRequest = new LogoutRequest("", null) {
                ClientIds = new List<string>() { "clientId1", "clientId2" }
            };
            accountServiceMock.Setup(x => x.BuildLoggedOutViewModelAsync(It.IsAny<string>()))
                .ReturnsAsync(new LoggedOutViewModel() {
                    LogoutId = model.LogoutId
                });
            idsServiceMock.Setup(s => s.GetLogoutContextAsync(model.LogoutId)).Returns(Task.FromResult(logoutRequest));
            eventsService.Setup(s => s.RaiseAsync(It.IsAny<IdentityServer4.Events.UserLogoutSuccessEvent>())).Returns(Task.CompletedTask);
            persistedGrantServiceMock.Setup(s => s.RemoveAllGrantsAsync(userSub, "clientId1", null)).Returns(Task.CompletedTask);
            persistedGrantServiceMock.Setup(s => s.RemoveAllGrantsAsync(userSub, "clientId2", null)).Returns(Task.CompletedTask);

            // act
            var result = await accountController.Logout(model);

            // assert
            Assert.NotNull(result);
            idsServiceMock.VerifyAll();
            persistedGrantServiceMock.VerifyAll();
            eventsService.VerifyAll();
        }

        //        [Fact(Skip = "fails without setting cookies -- see TODO")]
        //        public async Task ShouldBeAbleToLogin_Valid_ReturnURL() {
        //            //Arrange
        //            var model = CreateTestData<LoginModel>();
        //            Arrange(() => {
        //                model.RememberLogin = false;
        //                var user = CreateTestData<User>();
        //                authenticatorMock.Setup(x => x.AuthenticateAsync(It.IsAny<LoginInfo>()))
        //                    .ReturnsAsync(user);
        //                authManagerMock.Setup(x => x.SignInAsync(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), null))
        //                    .Returns(Task.FromResult(0));
        //                idsServiceMock.Setup(x => x.IsValidReturnUrl(model.ReturnUrl)).Returns(true);
        //            });

        //            //Act 
        //            var result = await accountController.Login(model) as RedirectResult;

        //            //Assert
        //            authenticatorMock.Verify(x => x.AuthenticateAsync(It.Is<LoginInfo>(l => l.Username == model.Username && l.Password == model.Password)));
        //            Assert.Equal(model.ReturnUrl, result.Url);
        //        }

        //        [Fact(Skip = "fails without setting cookies -- see TODO")]
        //        public async Task ShouldBeAbleToLogin_Invalid_ReturnURL() {
        //            //Arrange
        //            var model = CreateTestData<LoginInputModel>();
        //            var defaultUrl = Guid.NewGuid().ToString();
        //            Arrange(() => {
        //                model.RememberLogin = true;
        //                var user = CreateTestData<User>();
        //                authenticatorMock.Setup(x => x.AuthenticateAsync(It.IsAny<LoginInfo>()))
        //                    .ReturnsAsync(user);
        //                authManagerMock.Setup(x => x.SignInAsync(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
        //                    .Returns(Task.FromResult(0));
        //                idsServiceMock.Setup(x => x.IsValidReturnUrl(model.ReturnUrl)).Returns(false);
        //                configMock.Setup(x => x["defaultUrl"]).Returns(defaultUrl);
        //            });

        //            //Act 
        //            var result = await accountController.Login(model) as RedirectResult;

        //            //Assert
        //            authenticatorMock.Verify(x => x.AuthenticateAsync(It.Is<LoginInfo>(l => l.Username == model.Username && l.Password == model.Password)));
        //            Assert.Equal(defaultUrl, result.Url);
        //        }

        //        [Fact]
        //        public async Task ShouldBeAbleToLogin_Failed() {
        //            //Arrange
        //            var model = CreateTestData<LoginInputModel>();
        //            Arrange(() => {
        //                model.RememberLogin = true;
        //                authenticatorMock.Setup(x => x.AuthenticateAsync(It.IsAny<LoginInfo>()))
        //                    .ReturnsAsync(null as User);
        //            });

        //            //Act
        //            await accountController.Login(model);

        //            //Assert
        //            Assert.Equal(1, accountController.ModelState.ErrorCount);
        //        }

        //        [Fact(Skip = "fails without setting cookies -- see TODO")]
        //        public async Task ShouldBeAbleToLogout() {
        //            //Arrange
        //            var defaultUrl = Guid.NewGuid().ToString();
        //            Arrange(() => {
        //                configMock.Setup(x => x["defaultUrl"]).Returns(defaultUrl);
        //            });

        //            //Act
        //            var result = await accountController.Logout() as RedirectResult;

        //            //Assert
        //            authManagerMock.Verify(x => x.SignOutAsync("Cookies"));
        //            Assert.Equal(defaultUrl, result.Url);
        //        }

        [Fact]
        public async Task ShouldBeAbleToLogin_FailedMissingUsername() {
            //Arrange
            var model = CreateTestData<LoginInputModel>();
            Arrange(() => {
                model.Username = "";
                model.Password = "test";
                accountServiceMock.Setup(x => x.BuildLoginViewModelAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new LoginViewModel());
            });

            //Act
            await accountController.Login(model, "login");

            //Assert
            Assert.Equal(1, accountController.ModelState.ErrorCount);
        }

        [Fact]
        public async Task ShouldBeAbleToLogin_FailedMissingPassword() {
            //Arrange
            var model = CreateTestData<LoginInputModel>();
            Arrange(() => {
                model.Username = "test";
                model.Password = "";
                accountServiceMock.Setup(x => x.BuildLoginViewModelAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(new LoginViewModel());
            });

            //Act
            await accountController.Login(model, "login");
            Console.WriteLine(accountController.ModelState);
            //Assert
            Assert.Equal(1, accountController.ModelState.ErrorCount);
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

            /*return new ServiceCollection()
                .AddSingleton(new Mock<ILoggerFactory>().Object)
                .AddSingleton(authServiceMock)
                .AddSingleton(idsOptions)
                .AddSingleton(dicMock.Object)
                .BuildServiceProvider();*/
        }
    }
}
