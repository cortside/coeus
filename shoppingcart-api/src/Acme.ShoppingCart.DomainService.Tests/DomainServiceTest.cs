using System;
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
#pragma warning disable S2325
        protected DatabaseContext GetDatabaseContext() {
#pragma warning restore S2325
            var databaseContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase($"db-{Guid.NewGuid():d}")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new DatabaseContext(databaseContextOptions, new SubjectPrincipal([new Claim("sub", Guid.NewGuid().ToString())]), new DefaultSubjectFactory());
        }

        protected T Service { get; set; }
        protected Mock<IHttpContextAccessor> HttpContextAccessorMock { get; set; } = new Mock<IHttpContextAccessor>();

        public void SetupHttpUser(Claim claim) {
            Mock<HttpContext> httpContext = new Mock<HttpContext>();
            Mock<ClaimsPrincipal> user = new Mock<ClaimsPrincipal>();
            if (claim != null) {
                httpContext.SetupGet(x => x.User).Returns(user.Object);
                HttpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(httpContext.Object);
                user.Setup(x => x.FindFirst(claim.Type)).Returns(claim);
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
        }
    }
}
