using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Services;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Client = IdentityServer4.Models.Client;

namespace Acme.IdentityServer.WebApi.Tests.Services {

    public class UserProfileServiceTest : BaseTestFixture {
        private UserProfileService target;
        private Mock<IIdentityServerDbContext> dbCtxMock;
        private Mock<IServiceProvider> mockServiceProvider;

        public UserProfileServiceTest() {
            dbCtxMock = new Mock<IIdentityServerDbContext>();
            mockServiceProvider = new Mock<IServiceProvider>();

            Mock<IServiceScopeFactory> scopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactory.Object);
            Mock<IServiceScope> scope = new Mock<IServiceScope>();
            scopeFactory.Setup(s => s.CreateScope()).Returns(scope.Object);
            Mock<IServiceProvider> scopeServiceProvider = new Mock<IServiceProvider>();
            scope.Setup(s => s.ServiceProvider).Returns(scopeServiceProvider.Object);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IIdentityServerDbContext))).Returns(dbCtxMock.Object);

            target = new UserProfileService(mockServiceProvider.Object);
        }

        [Fact]
        public async Task ShouldBeAbleToGetProfileDataAsync() {
            //Arrange
            var user = CreateTestData<User>();
            user.UserClaims = new List<UserClaim> {
                new UserClaim() {Type="email", Value = "foo@bar.baz"}
            };

            var ctx = new ProfileDataRequestContext {
                Subject = CreatePrincipal(user),
                RequestedClaimTypes = new string[] { },
                ValidatedRequest = new IdentityServer4.Validation.ValidatedRequest() { ClientClaims = new List<Claim>() }
            };

            Arrange(() => {
                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
                ArrangeUserLists(user);
            });

            //Act
            await target.GetProfileDataAsync(ctx);

            //Assert
            Assert.Equal(6, ctx.IssuedClaims.Count);
            Assert.Equal(user.UserId.ToString(), ctx.IssuedClaims.First(x => x.Type == "sub").Value);
            Assert.Equal(user.Username, ctx.IssuedClaims.First(x => x.Type == "name").Value);
            Assert.Equal(user.Username, ctx.IssuedClaims.First(x => x.Type == "upn").Value);
            Assert.Contains(ctx.IssuedClaims, x => x.Type == "subject_type" && x.Value == "user");
            Assert.Contains(ctx.IssuedClaims, x => x.Type == "email" && x.Value == "foo@bar.baz");
            Assert.Contains(ctx.IssuedClaims, x => x.Type == "ip_address");
        }

        [Fact]
        public async Task ShouldBeAbleToGetIsActiveAsync_Active() {
            //Arrange
            var user = CreateTestData<User>();
            var ctx = new IsActiveContext(CreatePrincipal(user), new Client(), "asdf");
            Arrange(() => {
                user.UserStatus = IdsDefinitions.Active;
                user.EffectiveDate = DateTime.UtcNow.AddDays(-1);
                user.ExpirationDate = DateTime.UtcNow.AddDays(1);

                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            await target.IsActiveAsync(ctx);

            //Assert
            Assert.True(ctx.IsActive);
        }

        [Fact]
        public async Task ShouldBeAbleToGetIsActiveAsync_Active_NoExpiration() {
            //Arrange
            var user = CreateTestData<User>();
            var ctx = new IsActiveContext(CreatePrincipal(user), new Client(), "asdf");
            Arrange(() => {
                user.UserStatus = IdsDefinitions.Active;
                user.EffectiveDate = DateTime.UtcNow.AddDays(-1);
                user.ExpirationDate = null;

                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            await target.IsActiveAsync(ctx);

            //Assert
            Assert.True(ctx.IsActive);
        }

        [Fact]
        public async Task ShouldBeAbleToGetIsActiveAsync_InActive() {
            //Arrange
            var user = CreateTestData<User>();
            var ctx = new IsActiveContext(CreatePrincipal(user), new Client(), "asdf");
            Arrange(() => {
                user.UserStatus = IdsDefinitions.Inactive;
                user.EffectiveDate = DateTime.UtcNow.AddDays(-1);
                user.ExpirationDate = DateTime.UtcNow.AddDays(1);

                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            await target.IsActiveAsync(ctx);

            //Assert
            Assert.False(ctx.IsActive);
        }

        [Fact]
        public async Task ShouldBeAbleToGetIsActiveAsync_InActive_NotEffective() {
            //Arrange
            var user = CreateTestData<User>();
            var ctx = new IsActiveContext(CreatePrincipal(user), new Client(), "asdf");
            Arrange(() => {
                user.UserStatus = IdsDefinitions.Active;
                user.EffectiveDate = DateTime.UtcNow.AddDays(1);
                user.ExpirationDate = DateTime.UtcNow.AddDays(2);

                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            await target.IsActiveAsync(ctx);

            //Assert
            Assert.False(ctx.IsActive);
        }

        [Fact]
        public async Task ShouldBeAbleToGetIsActiveAsync_InActive_Expired() {
            //Arrange
            var user = CreateTestData<User>();
            var ctx = new IsActiveContext(CreatePrincipal(user), new Client(), "asdf");
            Arrange(() => {
                user.UserStatus = IdsDefinitions.Active;
                user.EffectiveDate = DateTime.UtcNow.AddDays(-2);
                user.ExpirationDate = DateTime.UtcNow.AddDays(-1);

                var usersMock = MockAsyncQueryable(user);
                dbCtxMock.Setup(x => x.Users).Returns(usersMock.Object);
            });

            //Act
            await target.IsActiveAsync(ctx);

            //Assert
            Assert.False(ctx.IsActive);
        }

        private ClaimsPrincipal CreatePrincipal(User user) {
            return new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim("sub", user.UserId.ToString())
            }));
        }

        private void ArrangeUserLists(User user) {
        }
    }
}
