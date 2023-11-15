using System;
using System.Collections.Generic;
using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Assemblers.Implementors;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models.Output;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Assemblers {
    public class UserModelAssemblerTests {
        private readonly UserModelAssembler assembler = null;

        public UserModelAssemblerTests() {
            assembler = new UserModelAssembler();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToModel(bool includeClaims) {
            var userId = new Guid();
            List<UserClaim> claims = null;

            if (includeClaims) {
                claims = new List<UserClaim>();
                claims.Add(new UserClaim() {
                    ProviderName = "provider",
                    User = new User(),
                    UserClaimId = 1,
                    UserId = userId,
                    Type = "type",
                    Value = "value"
                });
            }

            var isActive = true;

            //arrange
            string reason = "System hard locked the user due to too many failed login attempts.";
            User user = new User {
                CreateDate = DateTime.Now,
                CreateUserId = new Guid(),
                EffectiveDate = DateTime.Now,
                ExpirationDate = DateTime.Now,
                LastLogin = DateTime.Now,
                LastLoginIPAddress = string.Empty,
                LastModifiedDate = DateTime.Now,
                LastModifiedUserId = new Guid(),
                LockedReason = reason,
                Password = "password",
                TermsOfUseAcceptanceDate = DateTime.Now,
                Salt = "salt",
                UserId = userId,
                Username = "username",
                UserStatus = isActive ? "Active" : "Inactive",
                UserRoles = new List<UserRole>(),
                UserClaims = claims
            };

            //act
            UserOutputModel result = assembler.ToUserOutputModel(user);

            //assert
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.ProviderName, result.ProviderName);
            Assert.Equal(user.ProviderSubjectId, result.ProviderSubjectId);
            Assert.Equal(isActive, result.IsActive);
            Assert.Equal(reason, result.LockedReason);
            Assert.Equal(user.LastModifiedUserId, result.LastModifiedBySubjectId);
            if (includeClaims) {
                Assert.Equal(claims[0].Type, result.Claims[0].Type);
                Assert.Equal(claims[0].Value, result.Claims[0].Value);
            } else {
                Assert.Empty(result.Claims);
            }
        }
    }
}
