using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Controllers.Account;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Services {

    public class AuthenticatorTest : BaseTestFixture {
        private readonly Authenticator target;
        private readonly Mock<IIdentityServerDbContext> dbCtxMock;
        private readonly Mock<IHashProvider> hashProviderMock;
        private readonly Mock<IServiceProvider> mockServiceProvider;
        private readonly Mock<IHttpContextAccessor> httpMock;

        public AuthenticatorTest() {
            httpMock = new Mock<IHttpContextAccessor>();
            hashProviderMock = new Mock<IHashProvider>();
            dbCtxMock = new Mock<IIdentityServerDbContext>();
            mockServiceProvider = new Mock<IServiceProvider>();
            Mock<IServiceScopeFactory> scopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactory.Object);
            Mock<IServiceScope> scope = new Mock<IServiceScope>();
            scopeFactory.Setup(s => s.CreateScope()).Returns(scope.Object);
            Mock<IServiceProvider> scopeServiceProvider = new Mock<IServiceProvider>();
            scope.Setup(s => s.ServiceProvider).Returns(scopeServiceProvider.Object);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IIdentityServerDbContext))).Returns(dbCtxMock.Object);

            target = new Authenticator(httpMock.Object, hashProviderMock.Object, mockServiceProvider.Object);
        }

        [Fact]
        public async Task ShouldBeAbleToAuthenticate() {
            //Arrange
            var info = CreateTestData<LoginInfo>();
            var user = CreateTestData<User>();
            Arrange(() => {
                user.Username = info.Username;
                user.UserStatus = "Active";
                user.TwoFactorVerified = false;
                hashProviderMock.Setup(x => x.ComputeHash(info.Password + user.Salt)).Returns(user.Password);
                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            var result = await target.AuthenticateAsync(info);

            //Assert
            Assert.Same(user, result.User);
        }

        [Fact]
        public async Task ShouldBeAbleToAuthenticate_WrongPassword() {
            //Arrange
            var info = CreateTestData<LoginInfo>();
            Arrange(() => {
                var user = CreateTestData<User>();
                user.Username = info.Username;
                hashProviderMock.Setup(x => x.ComputeHash(info.Password)).Returns(Guid.NewGuid().ToString());
                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            var result = await target.AuthenticateAsync(info);

            //Assert
            Assert.Null(result.User);
            Assert.Equal(AccountOptions.InvalidCredentialsErrorMessage, result.Error);
        }

        [Fact]
        public async Task ShouldBeAbleToAuthenticate_UserNotFound() {
            //Arrange
            var info = CreateTestData<LoginInfo>();
            Arrange(() => {
                var user = CreateTestData<User>();
                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            var result = await target.AuthenticateAsync(info);

            //Assert
            Assert.Null(result.User);
            Assert.Equal(AccountOptions.InvalidCredentialsErrorMessage, result.Error);
        }

        [Fact]
        public async Task ShouldBeAbleToAuthenticate_UserLockedOn10thAttempt() {
            //Arrange
            var info = CreateTestData<LoginInfo>();
            Arrange(() => {
                var user = CreateTestData<User>();
                user.UserStatus = "Active";
                user.Username = info.Username;
                user.LoginAttempts = new List<LoginAttempt>();
                DateTime firstAttemptTime = DateTime.Now.AddDays(-1).AddSeconds(1);
                LoginAttempt firstAttempt = CreateTestData<LoginAttempt>();
                firstAttempt.AttemptedOn = firstAttemptTime;
                user.LoginAttempts.Add(firstAttempt);
                for (int i = 0; i < 8; i++) {
                    user.LoginAttempts.Add(CreateTestData<LoginAttempt>());
                }
                user.IsLocked = false;
                hashProviderMock.Setup(x => x.ComputeHash(info.Password)).Returns(Guid.NewGuid().ToString());
                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            var result = await target.AuthenticateAsync(info);

            //Assert request unsuccessful and error is locked
            Assert.Null(result.User);
            Assert.Equal(AccountOptions.UserLockedErrorMessage, result.Error);
        }

        [Fact]
        public async Task ShouldBeAbleToAuthenticate_UserPreviouslyLocked() {
            //Arrange
            var info = CreateTestData<LoginInfo>();
            Arrange(() => {
                var user = CreateTestData<User>();
                user.Username = info.Username;
                user.LoginAttempts = new List<LoginAttempt>();
                user.IsLocked = true;
                user.UserStatus = "Active";
                hashProviderMock.Setup(x => x.ComputeHash(info.Password)).Returns(Guid.NewGuid().ToString());
                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            var result = await target.AuthenticateAsync(info);

            //Assert request unsuccessful and error is locked
            Assert.Null(result.User);
            Assert.Equal(AccountOptions.UserLockedErrorMessage, result.Error);
        }

        [Theory]
        [InlineData("New", true)]
        [InlineData("New", false)]
        [InlineData("InActive", true)]
        [InlineData("InActive", false)]
        public async Task ShouldNotBeAbleToAuthenticateWhenUserNotActive(string status, bool locked) {
            // arrange
            var info = CreateTestData<LoginInfo>();
            var user = CreateTestData<User>();
            Arrange(() => {
                user.Username = info.Username;
                user.LoginAttempts = new List<LoginAttempt>();
                user.UserStatus = status;
                user.IsLocked = locked;
                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            var expectedFailedAttempt = new LoginAttempt {
                AttemptedOn = DateTime.Now,
                Successful = false,
                UserId = user.UserId
            };

            // act
            var result = await target.AuthenticateAsync(info);

            // assert
            result.User.Should().BeNull();
            result.Error.Should().BeEquivalentTo(AccountOptions.InvalidCredentialsErrorMessage);
            user.LoginAttempts.Should().BeEquivalentTo(new List<LoginAttempt>() { expectedFailedAttempt }, opt => opt.Excluding(prop => prop.AttemptedOn));
            user.LoginAttempts.First().AttemptedOn.Date.Should().Be(expectedFailedAttempt.AttemptedOn.Date);
            hashProviderMock.Verify(provider => provider.ComputeHash(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ShouldBeAbleToAuthenticate_UserNotLockedBeyond24h() {
            //Arrange
            var info = CreateTestData<LoginInfo>();
            var user = CreateTestData<User>();
            Arrange(() => {
                user.Username = info.Username;
                user.LoginAttempts = new List<LoginAttempt>();
                DateTime firstAttemptTime = DateTime.Now.AddDays(-1).AddSeconds(-1);
                LoginAttempt firstAttempt = CreateTestData<LoginAttempt>();
                firstAttempt.AttemptedOn = firstAttemptTime;
                user.LoginAttempts.Add(firstAttempt);
                for (int i = 0; i < 8; i++) {
                    user.LoginAttempts.Add(CreateTestData<LoginAttempt>());
                }
                user.IsLocked = false;
                hashProviderMock.Setup(x => x.ComputeHash(info.Password)).Returns(Guid.NewGuid().ToString());
                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            var result = await target.AuthenticateAsync(info);

            //Assert request unsuccessful and error is not locked
            Assert.Null(result.User);
            Assert.Equal(AccountOptions.InvalidCredentialsErrorMessage, result.Error);
        }

        [Fact]
        public async Task ShouldBeAbleToAuthenticate_RecentAttemptPassed() {
            //Arrange
            var info = CreateTestData<LoginInfo>();
            var user = CreateTestData<User>();
            Arrange(() => {
                user.Username = info.Username;
                user.UserStatus = "Active";
                user.LoginAttempts = new List<LoginAttempt>();
                for (int i = 0; i < 9; i++) {
                    if (i == 4) {
                        LoginAttempt successfulAttempt = CreateTestData<LoginAttempt>();
                        successfulAttempt.Successful = true;
                        user.LoginAttempts.Add(successfulAttempt);
                    } else {
                        user.LoginAttempts.Add(CreateTestData<LoginAttempt>());
                    }
                }
                user.IsLocked = false;
                hashProviderMock.Setup(x => x.ComputeHash(info.Password + user.Salt)).Returns(user.Password);
                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            var result = await target.AuthenticateAsync(info);

            //Assert
            Assert.Same(user, result.User);
        }

    }
}
