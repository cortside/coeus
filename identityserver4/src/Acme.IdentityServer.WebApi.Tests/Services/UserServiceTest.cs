using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cortside.DomainEvent.EntityFramework;
using Acme.DomainEvent.Events;
using Acme.IdentityServer.WebApi.Controllers.Account;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Exceptions;
using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Services;
using FluentAssertions;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OtpNet;
using Serilog;
using Xunit;

namespace Acme.IdentityServer.WebApi.Tests.Services {

    public class UserServiceTest : BaseTestFixture {
        UserService service;
        Mock<IIdentityServerDbContext> idsDbContextMock;
        Mock<ILogger<UserService>> loggerMock;
        Mock<IHashProvider> hashProviderMock;
        private Mock<IDomainEventOutboxPublisher> domainEventPublisherMock;
        private readonly IConfiguration configuration;
        Mock<IServiceProvider> mockServiceProvider;
        private Mock<IHttpContextAccessor> httpMock;
        private StringWriter stringWriter;

        public UserServiceTest() {
            idsDbContextMock = InstantiateMock<IIdentityServerDbContext>();
            mockServiceProvider = new Mock<IServiceProvider>();
            loggerMock = InstantiateMock<ILogger<UserService>>();
            hashProviderMock = InstantiateMock<IHashProvider>();
            domainEventPublisherMock = InstantiateMock<IDomainEventOutboxPublisher>();
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>() {
                {"RestrictedEmailDomains:0", "Acmeusa.com"},
                {"RestrictedEmailDomains:1", "Acme.com"},
            });
            configuration = configurationBuilder.Build();
            SetupService(idsDbContextMock.Object);

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

            var logMessage = stringWriter.ToString();
            Assert.Contains($"User {user.Username} had password updated with success", logMessage);
        }

        [Fact]
        public async Task ShouldCheckDuplicateUsernameWhenUpdatingUserAsync() {
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
            await Assert.ThrowsAsync<InvalidValueMessage>(() => service.UpdateUser(userId, model));
        }

        [Fact]
        public async Task ShouldValidateExistenceWhenUpdatingUserAsync() {
            // arrange
            Guid userId = Guid.NewGuid();
            UpdateUserModel model = new UpdateUserModel() {
                Username = "bluesky"
            };
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>().AsQueryable());

            // act & assert
            await Assert.ThrowsAsync<ResourceNotFoundMessage>(() => service.UpdateUser(userId, model));
        }

        [Fact]
        public async Task ShouldUpdateUserAsync() {
            // arrange
            Guid userId = Guid.NewGuid();
            UpdateUserModel model = new UpdateUserModel() {
                Username = "bluesky",
                Claims = new List<UserClaimModel>()
                {
                    new UserClaimModel() { Type = "gender", Value = "female" },
                    new UserClaimModel() { Type = "given_name", Value = "Jane" },
                    new UserClaimModel() { Type = "family_name", Value = "Doe" },
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
            domainEventPublisherMock.Setup(d => d.PublishAsync(It.Is<UserStateChangedEvent>(u => u.SubjectId == userId && u.GivenName.Equals("Jane") && u.FamilyName.Equals("Doe")))).Returns(Task.CompletedTask);

            // act
            var result = await service.UpdateUser(userId, model);

            // assert
            Assert.Equal(model.Username, result.Username);
            Assert.Equal(model.Claims.Count, result.UserClaims.Count);
            Assert.Contains(result.UserClaims, c => c.Type == "gender" && c.Value == "female");
            Assert.Contains(result.UserClaims, c => c.Type == "given_name" && c.Value == "Jane");
            Assert.Contains(result.UserClaims, c => c.Type == "family_name" && c.Value == "Doe");

            var logMessage = stringWriter.ToString();
            Assert.Contains($"User {model.Username} was updated with success", logMessage);
        }

        [Fact]
        public void ShouldGenerate32Digit2FaKey() {
            // arrange
            Guid userId = Guid.NewGuid();
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

            // act
            var result = service.GenerateAndSetNewTOTPCode(userId);


            // assert
            Assert.Equal(32, result.Length);
        }

        [Fact]
        public void ShouldValidateGenerated2FaCodeWithSavedKey() {
            // arrange
            Guid userId = Guid.NewGuid();
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

            //generate code
            var resultCode = service.GenerateAndSetNewTOTPCode(userId);

            //use code to check what new key should be 
            Assert.Equal(32, resultCode.Length);
            var base32Bytes = Base32Encoding.ToBytes(resultCode);
            OtpNet.Totp totp = new Totp(base32Bytes);
            var currentTotp = totp.ComputeTotp();
            //check key with what is saved on user
            var validated = service.VerifyCurrentTOTP(userId, currentTotp);
            //if not validated, recalculate to check again just in case of time drift and rotation
            if (!validated) {
                currentTotp = totp.ComputeTotp();
                //check key with what is saved on user
                validated = service.VerifyCurrentTOTP(userId, currentTotp);
            }
            Assert.True(validated);
        }

        [Fact]
        public async Task ShouldValidateUserExistenceWhenUpdatingUserAsync() {
            // arrange
            Guid subjectId = Guid.NewGuid();
            UpdateUserModel model = new UpdateUserModel();
            idsDbContextMock.Setup(c => c.Users).Returns(new List<User>().AsQueryable());

            // act & assert
            await Assert.ThrowsAsync<ResourceNotFoundMessage>(() => service.UpdateUser(subjectId, model));
        }

        [Fact]
        public async Task ShouldValidateDuplicateUsernameWhenUpdatingUserAsync() {
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
        public async Task ShouldCheckDuplicateUsernameWhenCreatingUserAsync() {
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
        public async Task ShouldCheckAcmeEmailWhenCreatingUserAsync() {
            // arrange
            CreateUserModel model = new CreateUserModel() {
                Username = "username",
                Claims = new List<UserClaimModel>() {
                    new UserClaimModel() {
                        Type = "email",
                        Value = "tester@Acmeusa.com"
                    }
                }
            };

            // act & assert
            var result = await Assert.ThrowsAsync<InvalidEmailMessage>(() => service.CreateUser(model));
        }

        [Fact]
        public async Task ShouldCreateUserAsync() {
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
            Assert.Equal("New", actual.UserStatus);
            foreach (var claim in model.Claims) {
                Assert.Contains(actual.UserClaims, c => c.Type == claim.Type && c.Value == claim.Value && c.ProviderName == null);
            }
            Assert.NotEqual(DateTime.MinValue, actual.EffectiveDate);
            Assert.NotEqual(DateTime.MinValue, actual.TermsOfUseAcceptanceDate);
        }

        [Fact]
        public async Task ShouldAutoProvisionUserAsync() {
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
            Assert.Equal(5, actual.UserClaims.Count);
            foreach (var claim in claims.Where(c => c.Type != "upn")) {
                Assert.Contains(actual.UserClaims, c => c.Type == claim.Type && c.Value == claim.Value && c.ProviderName == provider);
            }
            Assert.Contains(actual.UserClaims, c => c.Type == JwtClaimTypes.Name && c.Value == "givenname" && c.ProviderName == provider);
            Assert.Contains(actual.UserClaims, c => c.Type == "ip_address" && c.ProviderName == provider);
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

        [Fact]
        public async Task ShouldGetUserAsync() {
            var context = GetDatabaseContext();
            SetupService(context);

            var userId = Guid.NewGuid();

            await context.Users.AddAsync(new User { UserId = userId });
            var logins = new List<LoginAttempt> {
                new LoginAttempt { UserId = userId, Successful = true, AttemptedOn = DateTime.UtcNow.AddSeconds(10) },
                new LoginAttempt { UserId = userId, Successful = true, AttemptedOn = DateTime.UtcNow.AddSeconds(12) },
                new LoginAttempt { UserId = Guid.NewGuid(), Successful = true, AttemptedOn = DateTime.UtcNow.AddSeconds(15) },
                new LoginAttempt { UserId = userId, Successful = false, AttemptedOn = DateTime.UtcNow.AddSeconds(20) },
            };
            await context.LoginAttempts.AddRangeAsync(logins);

            await context.SaveChangesAsync();

            var user = await service.FindBySubjectIdAsync(userId);

            user.Should().NotBeNull();
            user.UserId.Should().Be(userId);
            user.LastLogin.Should().Be(logins[1].AttemptedOn);
        }

        [Fact]
        public async Task ShouldGetUserWithNullLastLoginAsync() {
            var context = GetDatabaseContext();
            SetupService(context);

            var userId = Guid.NewGuid();

            await context.Users.AddAsync(new User { UserId = userId });
            var logins = new List<LoginAttempt> {
                new LoginAttempt { UserId = userId, Successful = false, AttemptedOn = DateTime.UtcNow.AddSeconds(20) },
            };
            await context.LoginAttempts.AddRangeAsync(logins);

            await context.SaveChangesAsync();

            var user = await service.FindBySubjectIdAsync(userId);

            user.Should().NotBeNull();
            user.UserId.Should().Be(userId);
            user.LastLogin.Should().BeNull();
        }

        [Fact]
        public async Task ShouldNotNotFoundGetUserAsync() {
            var context = GetDatabaseContext();
            SetupService(context);

            await service.Invoking(srv => srv.FindBySubjectIdAsync(It.IsAny<Guid>())).Should().ThrowAsync<ResourceNotFoundMessage>();
        }

        private void SetupService(IIdentityServerDbContext dbContext) {
            httpMock = new Mock<IHttpContextAccessor>();
            Mock<IServiceScopeFactory> scopeFactory = new Mock<IServiceScopeFactory>();
            mockServiceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory))).Returns(scopeFactory.Object);
            Mock<IServiceScope> scope = new Mock<IServiceScope>();
            scopeFactory.Setup(s => s.CreateScope()).Returns(scope.Object);
            Mock<IServiceProvider> scopeServiceProvider = new Mock<IServiceProvider>();
            scope.Setup(s => s.ServiceProvider).Returns(scopeServiceProvider.Object);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IIdentityServerDbContext))).Returns(dbContext);
            scopeServiceProvider.Setup(s => s.GetService(typeof(IDomainEventOutboxPublisher))).Returns(domainEventPublisherMock.Object);

            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Serilog.Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            ILoggerFactory factory = new LoggerFactory().AddSerilog(Log.Logger);
            var logger = factory.CreateLogger<UserService>();

            service = new UserService(httpMock.Object, hashProviderMock.Object, logger, configuration, mockServiceProvider.Object);
        }
    }
}
