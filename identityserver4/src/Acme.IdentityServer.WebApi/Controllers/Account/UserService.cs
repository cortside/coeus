using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.IdentityServer.WebApi.AuditEvents;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Events;
using Acme.IdentityServer.WebApi.Exceptions;
using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Models.Enumerations;
using Acme.IdentityServer.WebApi.Models.Input;
using Acme.IdentityServer.WebApi.Services;
using Cortside.Common.Validation;
using Cortside.DomainEvent.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OtpNet;

namespace Acme.IdentityServer.WebApi.Controllers.Account {
    public class UserService : IUserService {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly string[] restrictedEmailDomains;
        private readonly IServiceProvider serviceProvider;
        private readonly IHashProvider hashProvider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor, IHashProvider hashProvider, ILogger<UserService> logger, IConfiguration configuration, IServiceProvider serviceProvider) {
            this.httpContextAccessor = httpContextAccessor;
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
        public async Task<User> FindBySubjectIdAsync(Guid subjectId) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == subjectId).ConfigureAwait(false);

                Guard.Against<ResourceNotFoundMessage>(() => user == null);

                var lastLogin = await dbContext.LoginAttempts
                    .AsNoTracking()
                    .OrderByDescending(la => la.AttemptedOn)
                    .FirstOrDefaultAsync(x => x.UserId == user.UserId && x.Successful).ConfigureAwait(false);

                user.LastLogin = lastLogin?.AttemptedOn;

                return user;
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

                user.Update(userCreateRequest.Username, UserStatus.New, userCreateRequest.Claims ?? new List<UserClaimModel>(), restrictedEmailDomains.ToList());

                dbContext.AddUser(user);
                dbContext.SaveChanges();

                // TODO: log siem event for success
                Serilog.Log.ForContext<UserCreateAuditEvent>().Information("User {Username} was created with {Result}", user.Username, "success");

                await PublishUserStateChangedEventAsync(user);
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

                // TODO: log siem event for success
                Serilog.Log.ForContext<UserUpdateAuditEvent>().Information("User {Username} was updated with {Result}", user.Username, "success");

                await PublishUserStateChangedEventAsync(user);

                return user;
            }
        }

        /// <summary>
        /// Generates a new 2FA TOTP Code, and sets TwoFactor property
        /// </summary>
        /// <returns>New Base32 encoded string</returns>
        public string GenerateAndSetNewTOTPCode(Guid subjectId) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var user = dbContext.Users.FirstOrDefault(x => x.UserId == subjectId);
                if (user == null) {
                    throw new ResourceNotFoundMessage();
                }

                var key = KeyGeneration.GenerateRandomKey(20);
                var base32String = Base32Encoding.ToString(key);
                user.TwoFactor = base32String;
                user.TwoFactorVerified = false;
                dbContext.SaveChanges();
                return base32String;
            }
        }

        /// <summary>
        /// Gets URI for display of 2D Barcode for Key setup.  Key must already be generated
        /// </summary>
        /// <returns>uri string</returns>
        public string GetTwoFactorURI(Guid subjectId) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var user = dbContext.Users.FirstOrDefault(x => x.UserId == subjectId);
                if (user == null) {
                    throw new ResourceNotFoundMessage();
                }
                if (string.IsNullOrEmpty(user.TwoFactor))
                    return null;
                var base32Bytes = Base32Encoding.ToBytes(user.TwoFactor);
                var authenticatorUri = new OtpUri(OtpType.Totp, base32Bytes, user.Username, "Acme - Applications").ToString();
                return authenticatorUri;
            }

        }

        public bool VerifyCurrentTOTP(Guid subjectId, string code) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var user = dbContext.Users.FirstOrDefault(x => x.UserId == subjectId);
                if (user == null) {
                    throw new ResourceNotFoundMessage();
                }
                if (string.IsNullOrEmpty(user.TwoFactor))
                    return false;
                var base32Bytes = Base32Encoding.ToBytes(user.TwoFactor);
                var otp = new Totp(base32Bytes);
                var currentCode = otp.ComputeTotp();
                if (currentCode == code) {
                    user.TwoFactorVerified = true;
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
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

                // TODO: log siem event for success
                Serilog.Log.ForContext<UserLockAuditEvent>().Information("User {Username} was {Action} with {Result}", user.Username, model.IsLocked ? "locked" : "unlocked", "success");

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

                // TODO: log siem event for success
                Serilog.Log.ForContext<UserPasswordUpdateAuditEvent>().Information("User {Username} had password updated with {Result}", user.Username, "success");
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
            // TODO: look at combining with the UserRegisteredHandler

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

            var attempt = new LoginAttempt {
                AttemptedOn = DateTime.Now,
                IpAddress = GetUserIP(httpContextAccessor),
                Successful = true,
                UserId = user.UserId
            };
            user.LoginAttempts.Add(attempt);
            user.LastLogin = attempt.AttemptedOn;
            user.LastLoginIPAddress = attempt.IpAddress;
            user.LoginCount++;

            claims.Add(new Claim("ip_address", attempt.IpAddress ?? "127.0.0.1"));
            user.UpdateExternal(claims);

            logger.LogInformation($"About to create new user: {user.Username}");
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                dbContext.AddUser(user);
                dbContext.SaveChanges();
            }
            await PublishUserStateChangedEventAsync(user);

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

                // TODO: log siem event for success
                Serilog.Log.ForContext<UserDeactivateAuditEvent>().Information("User {Username} was deactivated with {Result}", user.Username, "success");
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
                var user = dbContext.Users.Include(x => x.UserClaims).First(x => x.ProviderName == provider && x.ProviderSubjectId == userId);

                var attempt = new LoginAttempt {
                    AttemptedOn = DateTime.Now,
                    IpAddress = GetUserIP(httpContextAccessor),
                    Successful = true,
                    UserId = user.UserId
                };
                user.LoginAttempts.Add(attempt);
                user.LastLogin = attempt.AttemptedOn;
                user.LastLoginIPAddress = attempt.IpAddress;
                user.LoginCount++;

                claims.Add(new Claim("ip_address", attempt.IpAddress));
                user.UpdateExternal(claims);

                dbContext.UpdateUser(user);
                dbContext.SaveChanges();
                await PublishUserStateChangedEventAsync(user);
                return user;
            }
        }

        private void SetUserPasswordValues(User user, string password) {
            string salt = hashProvider.GenerateSalt();
            string hashed = hashProvider.ComputeHash($"{password}{salt}");

            user.Salt = salt;
            user.Password = hashed;
        }

        private async Task PublishUserStateChangedEventAsync(User user) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var outboxPublisher = scope.ServiceProvider.GetRequiredService<IDomainEventOutboxPublisher>();
                var @event = new UserStateChangedEvent() {
                    SubjectId = user.UserId,
                    Name = user.UserClaims.FirstOrDefault(u => u.Type.ToLower() == "name")?.Value,
                    GivenName = user.UserClaims.FirstOrDefault(u => u.Type.ToLower() == "given_name")?.Value,
                    FamilyName = user.UserClaims.FirstOrDefault(u => u.Type.ToLower() == "family_name")?.Value,
                    UserPrincipalName = user.Username
                };
                logger.LogDebug("Sending UserStateChangedEvent with body {@UserStateChangedEvent}", @event);
                await outboxPublisher.PublishAsync(@event);
                await dbContext.SaveChangesAsync();
            }
        }

        public static string GetUserIP(IHttpContextAccessor httpContextAccessor) {
            string ipList = httpContextAccessor?.HttpContext?.Request?.Headers["X-FORWARDED-FOR"].ToString();

            if (!string.IsNullOrEmpty(ipList)) {
                return ipList.Split(',')[0];
            }

            var ip = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (httpContextAccessor?.HttpContext?.Request?.Headers?.ContainsKey("REMOTE_ADDR") == true) {
                ip = httpContextAccessor.HttpContext.Request.Headers["REMOTE_ADDR"].ToString();
            }
            return ip;
        }
    }
}
