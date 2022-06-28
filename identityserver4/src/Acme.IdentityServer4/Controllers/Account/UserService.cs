using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Events;
using Cortside.IdentityServer.Data;
using Cortside.IdentityServer.Exceptions;
using Cortside.IdentityServer.Services;
using Cortside.IdentityServer.WebApi.Models;
using Cortside.IdentityServer.WebApi.Models.Enumerations;
using Cortside.IdentityServer.WebApi.Models.Input;
using Cortside.IdentityServer.WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cortside.IdentityServer.Controllers.Account {
    public class UserService : IUserService {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly string[] restrictedEmailDomains;
        private readonly IServiceProvider serviceProvider;
        private readonly IHashProvider hashProvider;

        public UserService(IHashProvider hashProvider, ILogger<UserService> logger, IConfiguration configuration, IServiceProvider serviceProvider) {
            this.hashProvider = hashProvider;
            this.logger = logger;
            this.configuration = configuration;
            restrictedEmailDomains = configuration.GetSection("RestrictedEmailDomains").Get<string[]>();
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Finds the user by subject identifier.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <returns></returns>
        public User FindBySubjectId(Guid subjectId) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                return dbContext.Users.FirstOrDefault(x => x.UserId == subjectId);
            }
        }

        public async Task<User> CreateUser(CreateUserModel userCreateRequest) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();

                // validate username
                var existing = dbContext.Users.FirstOrDefault(u => u.Username == userCreateRequest.Username);
                if (existing != null) {
                    throw new InvalidValueMessage(nameof(userCreateRequest.Username), userCreateRequest.Username);
                }

                User user = new User() {
                    UserId = Guid.NewGuid(),
                    Username = userCreateRequest.Username,
                    EffectiveDate = DateTime.UtcNow,
                    TermsOfUseAcceptanceDate = DateTime.UtcNow
                };
                this.SetUserPasswordValues(user, userCreateRequest.Password);

                user.Update(userCreateRequest.Username, Enum.Parse<UserStatus>(configuration.GetValue<string>("DefaultNewUserStatus")), userCreateRequest.Claims ?? new List<UserClaimModel>(), restrictedEmailDomains.ToList());

                dbContext.AddUser(user);
                dbContext.SaveChanges();
                await PublishUserStateChangedEvent(user);
                return user;
            }
        }

        public async Task<User> UpdateUser(Guid userId, UpdateUserModel userUpdateRequest) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                // validate user
                var user = dbContext.Users.Include(u => u.UserClaims).FirstOrDefault(u => u.UserId == userId);
                if (user == null) {
                    throw new ResourceNotFoundMessage();
                }

                // validate username
                var existing = dbContext.Users.FirstOrDefault(u => u.Username == userUpdateRequest.Username && u.UserId != userId);
                if (existing != null) {
                    throw new InvalidValueMessage(nameof(userUpdateRequest.Username), userUpdateRequest.Username);
                }

                user.Update(userUpdateRequest.Username, userUpdateRequest.Status, userUpdateRequest.Claims ?? new List<UserClaimModel>(), restrictedEmailDomains.ToList());

                dbContext.SaveChanges();
                await PublishUserStateChangedEvent(user);

                return user;
            }
        }

        public User UpdateUserLock(Guid subjectId, UpdateLockModel model) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var user = dbContext.Users.FirstOrDefault(x => x.UserId == subjectId);
                if (user == null) {
                    throw new ResourceNotFoundMessage();
                }
                user.IsLocked = model.IsLocked;

                // if unlocking add a successful login to prevent an instant lock on the first failed attempt
                if (!model.IsLocked) {
                    user.LockedReason = string.Empty;
                    user.LoginAttempts.Add(new LoginAttempt {
                        AttemptedOn = DateTime.Now,
                        Successful = true,
                        UserId = user.UserId
                    });
                } else {
                    user.LockedReason = model.Reason;
                }
                dbContext.SaveChanges();
                return user;
            }
        }

        public void UpdatePassword(Guid subjectId, UpdatePasswordModel model) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var user = dbContext.Users.FirstOrDefault(x => x.UserId == subjectId);
                if (user == null) {
                    throw new ResourceNotFoundMessage();
                }
                SetUserPasswordValues(user, model.Password);
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Finds the user by external provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="providerSubjectId">The user identifier.</param>
        /// <returns></returns>
        public User FindByExternalProvider(string provider, string providerSubjectId) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                return dbContext.Users.FirstOrDefault(x => x.ProviderName == provider && x.ProviderSubjectId == providerSubjectId);
            }
        }

        /// <summary>
        /// Automatically provisions a user.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public async Task<User> AutoProvisionUser(string provider, string userId, List<Claim> claims) {
            /// TODO: look at combining with the UserRegisteredHandler

            // create a new unique subject id
            var sub = Guid.NewGuid();

            // create new user
            var user = new User {
                UserId = sub,
                ProviderName = provider,
                ProviderSubjectId = userId,

                // set to someting since values are required (TODO?)
                Password = "EXTERNAL",
                Salt = "",

                EffectiveDate = DateTime.UtcNow,
                UserStatus = "Active",
                TermsOfUseAcceptanceDate = DateTime.UtcNow,
            };
            user.UpdateExternal(claims);

            logger.LogInformation($"About to create new user: {user.Username}");
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                dbContext.AddUser(user);
                dbContext.SaveChanges();
            }
            await PublishUserStateChangedEvent(user);

            return user;
        }

        /// <summary>
        /// Activate/Deactivate the User by setting the user Status to either Active or Inactive
        /// </summary>
        /// <returns>Were we successful</returns>
        public void DeactivateUser(Guid userId) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                // Quick check to see if we can deactivate this user
                var user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null) {
                    logger.LogInformation($"Attempted to deactivate a non-existing user ({userId})");
                    throw new ResourceNotFoundMessage();
                }

                // Deactivate the User
                user.UserStatus = IdsDefinitions.Inactive;

                logger.LogInformation($"Deactivated user ({userId})");

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Automatically provisions a user.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public async Task<User> UpdateExternalUserClaims(string provider, string userId, List<Claim> claims) {
            logger.LogInformation($"About to update external user claims for ProviderSubjectId: {userId}");
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var user = dbContext.Users.Include(x => x.UserClaims).FirstOrDefault(x => x.ProviderName == provider && x.ProviderSubjectId == userId);

                user.UpdateExternal(claims);
                dbContext.UpdateUser(user);
                dbContext.SaveChanges();
                await PublishUserStateChangedEvent(user);
                return user;
            }
        }

        private void SetUserPasswordValues(User user, string password) {
            string salt = hashProvider.GenerateSalt();
            string hashed = hashProvider.ComputeHash($"{password}{salt}");

            user.Salt = salt;
            user.Password = hashed;
        }

        private async Task PublishUserStateChangedEvent(User user) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var outboxPublisher = scope.ServiceProvider.GetRequiredService<IDomainEventOutboxPublisher>();
                var @event = new UserStateChangedEvent() {
                    SubjectId = user.UserId,
                    Name = user.UserClaims.FirstOrDefault(u => u.Type.ToLower() == "name")?.Value,
                    GivenName = user.UserClaims.FirstOrDefault(u => u.Type.ToLower() == "firstname")?.Value,
                    FamilyName = user.UserClaims.FirstOrDefault(u => u.Type.ToLower() == "lastname")?.Value,
                    UserPrincipalName = user.Username
                };
                logger.LogDebug("Sending UserStateChangedEvent with body {@UserStateChangedEvent}", @event);
                await outboxPublisher.PublishAsync(@event);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
