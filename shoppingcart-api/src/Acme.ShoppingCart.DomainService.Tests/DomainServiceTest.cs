using System;
using System.Collections.Generic;
using System.Security.Claims;
using Acme.ShoppingCart.Data;
using Cortside.AspNetCore.Auditable;
using Cortside.Common.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace Acme.ShoppingCart.DomainService.Tests {
    public abstract class DomainServiceTest<T> : IDisposable {
        protected T service;
        protected UnitTestFixture testFixture;
        protected readonly Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        protected DomainServiceTest() {
            testFixture = new UnitTestFixture();
        }

        protected DatabaseContext GetDatabaseContext() {
            var databaseContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase($"db-{Guid.NewGuid():d}")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new DatabaseContext(databaseContextOptions, new SubjectPrincipal(new List<Claim>() { new Claim("sub", Guid.NewGuid().ToString()) }), new DefaultSubjectFactory());
            return context;
        }

        public void SetupHttpUser(Claim claim) {
            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            Mock<ClaimsPrincipal> user = new Mock<ClaimsPrincipal>();
            if (claim != null) {
                httpContext.SetupGet(x => x.User).Returns(user.Object);
                httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(httpContext.Object);
                user.Setup(x => x.FindFirst(claim.Type)).Returns(claim);
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            testFixture.TearDown();
        }
    }
}
