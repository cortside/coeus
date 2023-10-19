using System;
using System.Collections.Generic;
using System.Security.Claims;
using Cortside.Common.Security;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Security {
    public class SubjectPrincipalTest {
        [Fact]
        public void ShouldGetSubjectId() {
            // arrrange
            Guid subjectId = Guid.NewGuid();
            ClaimsPrincipal claimsPrincipal = GetClaimsPrincipal(subjectId, new List<Claim>());
            SubjectPrincipal subjectPrincipal = new SubjectPrincipal(claimsPrincipal);

            // act
            var actual = subjectPrincipal.SubjectId;

            // assert
            Assert.Equal(subjectId.ToString(), actual);
        }

        private ClaimsPrincipal GetClaimsPrincipal(Guid subjectId, List<Claim> claims) {
            List<Claim> allClaims = new List<Claim>(claims);
            allClaims.Add(new Claim("sub", subjectId.ToString()));
            var identity = new ClaimsIdentity(allClaims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return claimsPrincipal;
        }
    }
}
