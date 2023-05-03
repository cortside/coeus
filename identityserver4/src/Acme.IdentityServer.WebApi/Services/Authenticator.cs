using System;
using System.Linq;
using System.Threading.Tasks;
using Acme.IdentityServer.Controllers.Account;
using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Models.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.IdentityServer.Services {
    public class Authenticator : IAuthenticator {
        readonly IHashProvider hashProvider;
        private readonly IServiceProvider serviceProvider;
        private readonly IHttpContextAccessor httpContextAccessor;

        public Authenticator(IHttpContextAccessor httpContextAccessor, IHashProvider hashProvider, IServiceProvider serviceProvider) {
            this.httpContextAccessor = httpContextAccessor;
            this.hashProvider = hashProvider;
            this.serviceProvider = serviceProvider;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(LoginInfo info) {
            using (var scope = serviceProvider.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
                var user = await dbContext.Users.Include(x => x.LoginAttempts).FirstOrDefaultAsync(x => x.Username == info.Username);
                if (user == null) {
                    return new AuthenticationResponse() { User = null, Error = AccountOptions.InvalidCredentialsErrorMessage };
                }

                if (!string.Equals(user.UserStatus, UserStatus.Active.ToString())) {
                    user.LoginAttempts.Add(new LoginAttempt {
                        AttemptedOn = DateTime.Now,
                        IpAddress = GetUserIP(),
                        Successful = false,
                        UserId = user.UserId
                    });
                    await dbContext.SaveChangesAsync();
                    return new AuthenticationResponse() { User = null, Error = AccountOptions.InvalidCredentialsErrorMessage };
                }

                if (user.IsLocked) {
                    user.LoginAttempts.Add(new LoginAttempt {
                        AttemptedOn = DateTime.Now,
                        IpAddress = GetUserIP(),
                        Successful = false,
                        UserId = user.UserId
                    });
                    await dbContext.SaveChangesAsync();
                    return new AuthenticationResponse() { User = null, Error = AccountOptions.UserLockedErrorMessage };
                }

                var pwSalt = (info?.Password ?? string.Empty) + (user?.Salt ?? string.Empty);
                var pwHash = hashProvider.ComputeHash(pwSalt);
                var passwordMatches = user?.Password == pwHash;
                var attempt = new LoginAttempt {
                    AttemptedOn = DateTime.Now,
                    IpAddress = GetUserIP(),
                    Successful = passwordMatches,
                    UserId = user.UserId
                };
                user.LoginAttempts.Add(attempt);

                if (attempt.Successful) {
                    user.LastLogin = attempt.AttemptedOn;
                    user.LastLoginIPAddress = attempt.IpAddress;
                    user.LoginCount++;
                }

                var yesterday = DateTime.Now.AddDays(-1);
                var attempts = user.LoginAttempts
                                    .Where(a => a.AttemptedOn >= yesterday)
                                    .OrderByDescending(a => a.AttemptedOn)
                                    .Take(10);
                var shouldLock = (attempts.Count() == 10 && attempts.All(a => !a.Successful));
                user.IsLocked = shouldLock;

                await dbContext.SaveChangesAsync();

                string err = null;
                if (user.IsLocked) {
                    err = AccountOptions.UserLockedErrorMessage;
                } else {
                    err = !passwordMatches ? AccountOptions.InvalidCredentialsErrorMessage : null;
                }

                User returnUser = err == null ? user : null;

                return new AuthenticationResponse() { User = returnUser, Error = err };
            }
        }

        private string GetUserIP() {
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
