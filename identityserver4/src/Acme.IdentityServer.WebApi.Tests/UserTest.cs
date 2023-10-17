using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Exceptions;
using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Models.Enumerations;
using FluentAssertions;
using IdentityModel;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests {
    public class UserTest : BaseTestFixture {

        [Fact]
        public void ShouldUpdateUser() {
            // arrange
            User user = new User() {
                Username = "username",
                UserClaims = new List<UserClaim> {
                    new UserClaim() { Type = JwtClaimTypes.Name, Value = "Jane Doe" },
                    new UserClaim() { Type = JwtClaimTypes.Audience, Value = "audience" },
                    new UserClaim() { Type = JwtClaimTypes.BirthDate, Value = "1/1/1999" }
                },
            };
            string newUsername = "newUsername";
            var newStatus = UserStatus.New;
            List<UserClaimModel> claimModels = new List<UserClaimModel>() {
                new UserClaimModel() { Type = JwtClaimTypes.Name, Value = "Jane Doe" },
                new UserClaimModel() { Type = JwtClaimTypes.Audience, Value = "new audience" },
                new UserClaimModel() { Type = JwtClaimTypes.PhoneNumber, Value = "8012223434" }
            };

            var expectedUser = new User() {
                Username = newUsername,
                UserStatus = newStatus.ToString(),
            };

            // act
            user.Update(newUsername, newStatus, claimModels, new List<string>() { "Acmeusa.com", "Acme.com" });

            // assert
            user.Should().BeEquivalentTo(expectedUser, opt => opt.Excluding(usr => usr.UserClaims));
            CompareAll(user.UserClaims.ToList(), claimModels, (c, model) => c.Type == model.Type && c.Value == model.Value).Should().BeTrue();
        }

        [Theory]
        [InlineData(JwtClaimTypes.Subject)]
        [InlineData("upn")]
        public void ShouldFilterClaimsWhenUpdatingUser(string claimType) {
            // arrange
            User user = new User() {
                Username = "username",
                UserClaims = new List<UserClaim> { }
            };
            List<UserClaimModel> claimModels = new List<UserClaimModel>();
            claimModels.Add(new UserClaimModel() {
                Type = claimType,
                Value = "claimValue"
            });

            // act 
            Assert.Throws<InvalidValueMessage>(() => user.Update("username", null, claimModels, new List<string>() { "Acmeusa.com", "Acme.com" }));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldValidateUsernameWhenUpdatingUser(string newUsername) {
            // arrange
            User user = new User() {
                Username = "username",
                UserClaims = new List<UserClaim>()
            };
            List<UserClaimModel> claimModels = new List<UserClaimModel>();

            // act & assert
            Assert.Throws<InvalidValueMessage>(() => user.Update(newUsername, null, claimModels, new List<string>() { "Acmeusa.com", "Acme.com" }));
        }

        [Fact]
        public void ShouldUpdateClaimsWhenUpdatingExternalUser() {
            // arrange
            Guid userId = Guid.NewGuid();
            User user = new User() {
                UserId = userId,
                Username = userId.ToString("d"),
                UserClaims = new List<UserClaim>() {
                    new UserClaim() {
                        Type = JwtClaimTypes.Audience,
                        Value = "audience"
                    },
                    new UserClaim() {
                        Type = JwtClaimTypes.PhoneNumber,
                        Value = "8012223434"
                    },
                    new UserClaim() {
                        Type = JwtClaimTypes.Expiration,
                        Value = "1/1/1999"
                    }
                }
            };
            List<Claim> externalClaims = new List<Claim>() {
                new Claim(JwtClaimTypes.Audience, "new audience"),
                new Claim(JwtClaimTypes.PhoneNumber, "8012223434"),
                new Claim(JwtClaimTypes.JwtId, "12345")
            };

            // act
            user.UpdateExternal(externalClaims);

            // assert
            Assert.Equal(userId, user.UserId);
            Assert.Equal(userId.ToString("d"), user.Username);
            Assert.Equal(3, user.UserClaims.Count);
            foreach (var claim in externalClaims) {
                Assert.Contains(user.UserClaims, c => c.Type == claim.Type && c.Value == claim.Value);
            }
        }

        [Theory]
        [InlineData(ClaimTypes.Name, JwtClaimTypes.Name)]
        [InlineData(ClaimTypes.Surname, JwtClaimTypes.FamilyName)]
        public void ShouldTranslateClaimWhenUpdatingExternalUser(string inputClaimType, string expectedClaimType) {
            // arrange
            Guid userId = Guid.NewGuid();
            string claimValue = "claimValue";
            User user = new User() {
                UserId = userId,
                Username = userId.ToString("d"),
                UserClaims = new List<UserClaim>()
            };
            List<Claim> externalClaims = new List<Claim>() {
                new Claim(inputClaimType, claimValue)
            };

            // act
            user.UpdateExternal(externalClaims);

            // assert
            Assert.Equal(userId, user.UserId);
            Assert.Equal(userId.ToString("d"), user.Username);
            Assert.Contains(user.UserClaims, c => c.Type == expectedClaimType && c.Value == claimValue);
        }

        [Theory]
        [InlineData("Jane", null, "Jane")]
        [InlineData(null, "Doe", "Doe")]
        [InlineData("Jane", "Doe", "Jane Doe")]
        public void ShouldAddNameClaimWhenPossibleUponUpdatingExternalUser(string firstName, string lastName, string expectedNameClaimValue) {
            // arrange
            Guid userId = Guid.NewGuid();
            User user = new User() {
                UserId = userId,
                Username = userId.ToString("d"),
                UserClaims = new List<UserClaim>()
            };
            List<Claim> externalClaims = new List<Claim>();
            if (firstName != null) { externalClaims.Add(new Claim(JwtClaimTypes.GivenName, firstName)); }
            if (lastName != null) { externalClaims.Add(new Claim(JwtClaimTypes.FamilyName, lastName)); }

            // act
            user.UpdateExternal(externalClaims);

            // assert
            if (firstName != null) {
                Assert.Contains(user.UserClaims, c => c.Type == JwtClaimTypes.GivenName && c.Value == firstName);
            }
            if (lastName != null) {
                Assert.Contains(user.UserClaims, c => c.Type == JwtClaimTypes.FamilyName && c.Value == lastName);
            }
            Assert.Contains(user.UserClaims, c => c.Type == JwtClaimTypes.Name && c.Value == expectedNameClaimValue);
        }

        [Fact]
        public void ShouldFilterClaimsWhenUpdatingExternalUser() {
            // arrange
            User user = new User() {
                UserId = Guid.NewGuid(),
                Username = "test@test.com"
            };
            List<Claim> externalClaims = new List<Claim>() {
                new Claim(JwtClaimTypes.BirthDate, "1/1/1999"),
                new Claim(JwtClaimTypes.Gender, "female"),
                new Claim("upn", "test@test.com"),
                new Claim(JwtClaimTypes.Subject, "subject")
            };

            // act
            user.UpdateExternal(externalClaims);

            // assert
            Assert.Equal("test@test.com", user.Username);
            Assert.Equal(2, user.UserClaims.Count);
            Assert.Contains(user.UserClaims, c => c.Type == JwtClaimTypes.BirthDate && c.Value == "1/1/1999");
            Assert.Contains(user.UserClaims, c => c.Type == JwtClaimTypes.Gender && c.Value == "female");
        }

        [Theory]
        [InlineData("upnusername", null, "upnusername")]
        [InlineData(null, "email@claim.com", "email@claim.com")]
        [InlineData("upnusername", "email@claim.com", "upnusername")]
        [InlineData(null, null, "2de307bb-5be1-45a0-8b1d-235f930daec4")]
        public void ShouldUpdateUsernameWhenUpdatingExternalUser(string upnClaim, string emailClaim, string expectedUsername) {
            // arrange
            User user = new User() {
                UserId = Guid.Parse("2de307bb-5be1-45a0-8b1d-235f930daec4"),
                Username = "originalusername"
            };
            List<Claim> externalClaims = new List<Claim>();
            if (upnClaim != null) {
                externalClaims.Add(new Claim("upn", upnClaim));
            }
            if (emailClaim != null) {
                externalClaims.Add(new Claim(JwtClaimTypes.Email, emailClaim));
            }

            // act
            user.UpdateExternal(externalClaims);

            // assert
            Assert.Equal(expectedUsername, user.Username);
        }

        [Fact]
        public void ShoulSkipStatusUpdate() {
            // arrange
            User user = new User() {
                Username = "username",
                UserClaims = new List<UserClaim> {
                    new UserClaim() { Type = JwtClaimTypes.Name, Value = "Jane Doe" },
                    new UserClaim() { Type = JwtClaimTypes.Audience, Value = "audience" },
                    new UserClaim() { Type = JwtClaimTypes.BirthDate, Value = "1/1/1999" }
                },
                UserStatus = "Active"
            };
            string newUsername = "newUsername";
            List<UserClaimModel> claimModels = new List<UserClaimModel>() {
                new UserClaimModel() { Type = JwtClaimTypes.Name, Value = "Jane Doe" },
                new UserClaimModel() { Type = JwtClaimTypes.Audience, Value = "new audience" },
                new UserClaimModel() { Type = JwtClaimTypes.PhoneNumber, Value = "8012223434" }
            };

            var expectedUser = new User() {
                Username = newUsername,
                UserStatus = "Active",
            };

            // act
            user.Update(newUsername, null, claimModels, new List<string>() { "Acmeusa.com", "Acme.com" });

            // assert
            user.Should().BeEquivalentTo(expectedUser, opt => opt.Excluding(usr => usr.UserClaims));
            CompareAll(user.UserClaims.ToList(), claimModels, (c, model) => c.Type == model.Type && c.Value == model.Value).Should().BeTrue();
        }
    }
}
