using System;
using System.Collections.Generic;
using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Services {
    public class ScopeServiceTest : BaseClientTest {
        private ScopeService service;

        private Mock<IServiceProvider> mockServiceProvider;

        public ScopeServiceTest() {
            mockServiceProvider = new Mock<IServiceProvider>();
            Mock<IServiceScopeFactory> scopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactory.Object);
            Mock<IServiceScope> scope = new Mock<IServiceScope>();
            scopeFactory.Setup(s => s.CreateScope()).Returns(scope.Object);
            Mock<IServiceProvider> scopeServiceProvider = new Mock<IServiceProvider>();
            scope.Setup(s => s.ServiceProvider).Returns(scopeServiceProvider.Object);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IIdentityServerDbContext))).Returns(IdentityServerDbContext);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IdentityServerDbContext))).Returns(IdentityServerDbContext);

            service = new ScopeService(mockServiceProvider.Object);
        }

        [Fact]
        public void ShouldGetAllScopes() {
            // setup
            var scope1 = new ApiScope {
                DisplayName = "scope1",
                Enabled = true,
                Description = "scope1",
                Name = "scope1",
                ApiResourceId = 1,
                ApiScopeId = 0,
                Emphasize = false,
                Id = 1,
                Required = true
            };
            var scope2 = new ApiScope {
                DisplayName = "scope2",
                Enabled = true,
                Description = "scope2",
                Name = "scope2",
                ApiResourceId = 2,
                ApiScopeId = 0,
                Emphasize = false,
                Id = 2,
                Required = true
            };

            IdentityServerDbContext.ApiScopes.Add(scope1);
            IdentityServerDbContext.ApiScopes.Add(scope2);
            IdentityServerDbContext.SaveChanges();

            // act
            var result = service.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(new List<string> { "scope1", "scope2" }, result);
        }

        [Fact]
        public void ShouldGetEmptyList() {
            // setup

            // act
            var result = service.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(new List<string>(), result);
        }
    }
}
