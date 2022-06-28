using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Events;
using Cortside.IdentityServer.Controllers.Account;
using Cortside.IdentityServer.Data;
using Cortside.IdentityServer.Exceptions;
using Cortside.IdentityServer.Services;
using Cortside.IdentityServer.WebApi.Models;
using Cortside.IdentityServer.WebApi.Models.Input;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cortside.IdentityServer.Tests.Services {

    public class UserServiceTest : BaseTestFixture {
        UserService service;
        Mock<IIdentityServerDbContext> idsDbContextMock;
        Mock<ILogger<UserService>> loggerMock;
        Mock<IHashProvider> hashProviderMock;
        private Mock<IDomainEventOutboxPublisher> domainEventPublisherMock;
        private readonly IConfiguration configuration;
        Mock<IServiceProvider> mockServiceProvider;

        public UserServiceTest() {
            idsDbContextMock = InstantiateMock<IIdentityServerDbContext>();
            mockServiceProvider = new Mock<IServiceProvider>();
            loggerMock = InstantiateMock<ILogger<UserService>>();
            hashProviderMock = InstantiateMock<IHashProvider>();
            domainEventPublisherMock = InstantiateMock<IDomainEventOutboxPublisher>();
            Mock<IServiceScopeFactory> scopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactory.Object);
            Mock<IServiceScope> scope = new Mock<IServiceScope>();
            scopeFactory.Setup(s => s.CreateScope()).Returns(scope.Object);
            Mock<IServiceProvider> scopeServiceProvider = new Mock<IServiceProvider>();
            scope.Setup(s => s.ServiceProvider).Returns(scopeServiceProvider.Object);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IIdentityServerDbContext))).Returns(idsDbContextMock.Object);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IDomainEventOutboxPublisher))).Returns(domainEventPublisherMock.Object);
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>() {
                {"RestrictedEmailDomains:0", "cortsideusa.com"},
                {"RestrictedEmailDomains:1", "cortside.com"},
                { "DefaultNewUserStatus", "Active" }
            });
            configuration = configurationBuilder.Build();
            service = new UserService(hashProviderMock.Object, loggerMock.Object, configuration, mockServiceProvider.Object);

            hashProviderMock.VerifyAll();
        }

        [Fact]
        public void ShouldValidateUserExistenceWhenUpdatingPassword() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdatePasswordModel model = new UpdatePasswordModel();
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>().AsQueryable());

            // act & assert
            Assert.Throws<ResourceNotFoundMessage>(() => service.UpdatePassword(subjectId, model));
        }

        [Fact]
        public void ShouldUpdateUserPassword() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdatePasswordModel model = new UpdatePasswordModel() {
                Password = "mypassword"
            };
            User user = new User() {
                UserId = subjectId,
                Username = "username13",
                Salt = "originalsalt",
                Password = "originalpassword"
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>
            {
                user
            }.AsQueryable());
            string salt = "newsalt";
            string hashedPassword = "hashedPassword";
            hashProviderMock.Setup(h => h.GenerateSalt()).Returns(salt);
            hashProviderMock.Setup(h => h.ComputeHash(It.IsAny<string>())).Returns(hashedPassword);

            // act
            service.UpdatePassword(subjectId, model);

            // assert
            Assert.Equal(salt, user.Salt);
            Assert.Equal(hashedPassword, user.Password);
        }

        [Fact]
        public void ShouldCheckDuplicateUsernameWhenUpdatingUser() {
            // arrange
            Guid userId = Guid.NewGuid();
            UpdateUserModel model = new UpdateUserModel() {
                Username = "bluesky",
                Claims = new List<UserClaimModel>()
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>
            {
                new User(){ UserId = userId, Username = "username13" },
                new User(){ UserId = Guid.NewGuid(), Username = "bluesky" }
            }.AsQueryable());

            // act & assert
            Assert.ThrowsAsync<InvalidValueMessage>(() => service.UpdateUser(userId, model));
        }

        [Fact]
        public void ShouldValidateExistenceWhenUpdatingUser() {
            // arrange
            Guid userId = Guid.NewGuid();
            UpdateUserModel model = new UpdateUserModel() {
                Username = "bluesky"
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>().AsQueryable());

            // act & assert
            Assert.ThrowsAsync<ResourceNotFoundMessage>(() => service.UpdateUser(userId, model));
        }

        [Fact]
        public async Task ShouldUpdateUser() {
            // arrange
            Guid userId = Guid.NewGuid();
            UpdateUserModel model = new UpdateUserModel() {
                Username = "bluesky",
                Claims = new List<UserClaimModel>()
                {
                    new UserClaimModel() { Type = "gender", Value = "female" },
                    new UserClaimModel() { Type = "given_name", Value = "Jane Doe" }
                }
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>
            {
                new User() {
                    UserId = userId,
                    Username = "originalUsername",
                    UserClaims = new List<UserClaim>() {
                        new UserClaim() { Type = "name", Value = "originalUsername" },
                        new UserClaim() {Type = "aud", Value = "audadu"}
                    }
                }
            }.AsQueryable());
            idsDbContextMock.Setup(c => c.SaveChanges());
            domainEventPublisherMock.Setup(d => d.PublishAsync(It.Is<UserStateChangedEvent>(u => u.SubjectId == userId))).Returns(Task.CompletedTask);

            // act
            var result = await service.UpdateUser(userId, model);

            // assert
            Assert.Equal(model.Username, result.Username);
            Assert.Equal(model.Claims.Count, result.UserClaims.Count);
            Assert.Contains(result.UserClaims, c => c.Type == "gender" && c.Value == "female");
            Assert.Contains(result.UserClaims, c => c.Type == "given_name" && c.Value == "Jane Doe");
        }

        [Fact]
        public void ShouldValidateUserExistenceWhenUpdatingUser() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdateUserModel model = new UpdateUserModel();
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>().AsQueryable());

            // act & assert
            Assert.ThrowsAsync<ResourceNotFoundMessage>(() => service.UpdateUser(subjectId, model));
        }

        [Fact]
        public async Task ShouldValidateDuplicateUsernameWhenUpdatingUser() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdateUserModel model = new UpdateUserModel() {
                Username = "user123"
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>() {
                 new User() {
                    UserId = subjectId,
                    Username = "originalUsername"
                },
                  new User() {
                    UserId = Guid.NewGuid(),
                    Username = "user123"
                }
            }.AsQueryable());

            // act & assert
            var message = await Assert.ThrowsAsync<InvalidValueMessage>(() => service.UpdateUser(subjectId, model));
            Assert.Equal("Username", message.Name);
            Assert.Equal(model.Username, message.Value);
        }

        [Fact]
        public async Task ShouldCheckDuplicateUsernameWhenCreatingUser() {
            // arrange
            CreateUserModel model = new CreateUserModel() {
                Username = "username"
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>
            {
                new User() { Username = "username" }
            }.AsQueryable());

            // act
            var result = await Assert.ThrowsAsync<InvalidValueMessage>(() => service.CreateUser(model));

            // assert
            Assert.Equal("Username", result.Name);
            Assert.Equal(model.Username, result.Value);
        }

        [Fact]
        public async Task ShouldCheckCortsideEmailWhenCreatingUser() {
            // arrange
            CreateUserModel model = new CreateUserModel() {
                Username = "username",
                Claims = new List<UserClaimModel>() {
                    new UserClaimModel() {
                        Type = "email",
                        Value = "tester@cortsideusa.com"
                    }
                }
            };

            // act & assert
            var result = await Assert.ThrowsAsync<InvalidEmailMessage>(() => service.CreateUser(model));
        }

        [Fact]
        public async Task ShouldCreateUser() {
            // arrange
            CreateUserModel model = new CreateUserModel() {
                Username = "username",
                Password = "password",
                Claims = Enumerable.Range(1, 5).Select(i => new UserClaimModel() {
                    Type = "$Type{i}",
                    Value = (i * 5).ToString()
                }).ToList()
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>().AsQueryable());
            idsDbContextMock.Setup(c => c.AddUser(It.IsAny<User>()));
            idsDbContextMock.Setup(c => c.SaveChanges());
            string salt = "salt";
            string hashedPassword = "hashedPassword";
            hashProviderMock.Setup(h => h.GenerateSalt()).Returns(salt);
            hashProviderMock.Setup(h => h.ComputeHash(It.IsAny<string>())).Returns(hashedPassword);
            domainEventPublisherMock.Setup(d => d.PublishAsync(It.IsAny<UserStateChangedEvent>())).Returns(Task.CompletedTask);

            // act
            User actual = await service.CreateUser(model);

            // assert
            Assert.NotEqual(Guid.Empty, actual.UserId);
            Assert.Equal(model.Username, actual.Username);
            Assert.Equal(salt, actual.Salt);
            Assert.Equal(hashedPassword, actual.Password);
            Assert.Equal(model.Claims.Count, actual.UserClaims.Count);
            Assert.Equal("Active", actual.UserStatus);
            foreach (var claim in model.Claims) {
                Assert.Contains(actual.UserClaims, c => c.Type == claim.Type && c.Value == claim.Value && c.ProviderName == null);
            }
            Assert.NotEqual(DateTime.MinValue, actual.EffectiveDate);
            Assert.NotEqual(DateTime.MinValue, actual.TermsOfUseAcceptanceDate);
        }

        [Fact]
        public async Task ShouldAutoProvisionUser() {
            // arrange
            string provider = "provider";
            string userId = "userId";
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(JwtClaimTypes.Audience, "audience"));
            claims.Add(new Claim(JwtClaimTypes.BirthDate, "1/1/1975"));
            claims.Add(new Claim(JwtClaimTypes.GivenName, "givenname"));
            claims.Add(new Claim("upn", "myupn"));
            domainEventPublisherMock.Setup(d => d.PublishAsync(It.IsAny<UserStateChangedEvent>())).Returns(Task.CompletedTask);

            // act
            User actual = await service.AutoProvisionUser(provider, userId, claims);

            // assert
            Assert.Equal("myupn", actual.Username);
            Assert.Equal(provider, actual.ProviderName);
            Assert.Equal(userId, actual.ProviderSubjectId);
            Assert.Equal(4, actual.UserClaims.Count);
            foreach (var claim in claims.Where(c => c.Type != "upn")) {
                Assert.Contains(actual.UserClaims, c => c.Type == claim.Type && c.Value == claim.Value && c.ProviderName == provider);
            }
            Assert.Contains(actual.UserClaims, c => c.Type == JwtClaimTypes.Name && c.Value == "givenname" && c.ProviderName == provider);
        }

        [Fact]
        public void ShouldValidateExistenceWhenLockingUser() {
            // arrange
            Guid userId = Guid.NewGuid();
            UpdateLockModel model = new UpdateLockModel() {
                IsLocked = true
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>().AsQueryable());

            // act & assert
            Assert.Throws<ResourceNotFoundMessage>(() => service.UpdateUserLock(userId, model));
        }

        [Fact]
        public void ShouldLockUserLock() {
            // arrange
            string reason = "System hard locked the user due to too many failed login attempts.";

            Guid userId = Guid.NewGuid();
            UpdateLockModel model = new UpdateLockModel() {
                IsLocked = true,
                Reason = reason
            };
            User user = new User {
                UserId = userId,
                IsLocked = false
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>() { user }.AsQueryable());
            idsDbContextMock.Setup(c => c.SaveChanges());

            // act
            var result = service.UpdateUserLock(userId, model);

            // assert
            Assert.Equal(model.IsLocked, result.IsLocked);
            Assert.Equal(0, user.LoginAttempts.Count);
            Assert.Equal(reason, user.LockedReason);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldUnlockUserLock(bool doesUserHaveALockReason) {
            // arrange
            Guid userId = Guid.NewGuid();
            string oldLockReason = doesUserHaveALockReason ? "LockReason" : null;

            UpdateLockModel model = new UpdateLockModel() {
                IsLocked = false,
                Reason = "System hard locked the user due to too many failed login attempts."
            };
            User user = new User {
                UserId = userId,
                IsLocked = true,
                LockedReason = oldLockReason
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>() { user }.AsQueryable());
            idsDbContextMock.Setup(c => c.SaveChanges());

            // act
            var result = service.UpdateUserLock(userId, model);

            // assert
            Assert.Equal(model.IsLocked, result.IsLocked);
            Assert.Equal(1, user.LoginAttempts.Count);

            //Due to being an unlock the reason is cleared
            Assert.Empty(user.LockedReason);
        }
    }
}
