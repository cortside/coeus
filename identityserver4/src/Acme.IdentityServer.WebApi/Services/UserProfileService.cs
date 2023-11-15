using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.IdentityServer.WebApi.Services {
    public class UserProfileService : IProfileService {
        private readonly IServiceProvider serviceProvider;

        public UserProfileService(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context) {
            using IServiceScope scope = serviceProvider.CreateScope();
            IIdentityServerDbContext dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();

            var subjectId = context.Subject.GetSubjectId();
            var claims = context.Subject.Claims.ToList();

            // check to see if subject is a user
            User user = await GetUser(dbContext, subjectId);
            if (user != null) {
                claims.Remove(claims.FirstOrDefault(x => x.Type == "subject_type"));
                claims.Add(new Claim("subject_type", "user"));

                // TODO: should pass in context.RequestedClaimTypes to restrict returned claims
                claims.AddRange(BuildClaims(user, null));
            }

            if (!claims.Any(x => x.Type == JwtClaimTypes.Subject)) {
                claims.Add(new Claim(JwtClaimTypes.Subject, subjectId));
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context) {
            // TODO: how to know if user is external??
            using IServiceScope scope = serviceProvider.CreateScope();
            IIdentityServerDbContext dbContext = scope.ServiceProvider.GetRequiredService<IIdentityServerDbContext>();
            User user = await GetUser(dbContext, context.Subject.GetSubjectId());
            if (user != null) {
                DateTime now = DateTime.UtcNow;
                var inEffect = user.EffectiveDate <= now &&
                    (!user.ExpirationDate.HasValue || user.ExpirationDate >= now);
                context.IsActive = inEffect && user.UserStatus == IdsDefinitions.Active;
            } else {
                context.IsActive = true;
            }
        }

        private async Task<User> GetUser(IIdentityServerDbContext dbContext, string subjectId) {
            if (subjectId == null) {
                throw new InvalidOperationException("sub claim is missing.");
            }

            User user = await dbContext.Users
                .Include(x => x.UserClaims)
                .FirstOrDefaultAsync(x => x.UserId.ToString() == subjectId);
            return user;
        }

        private List<Claim> BuildClaims(User user, IEnumerable<string> requestedClaims) {
            var claims = new List<Claim>();

            // make sure username is the upn claim
            claims.Add(new Claim("upn", user.Username));

            // add claims from user
            foreach (var uc in user.UserClaims ?? new List<UserClaim>()) {
                claims.Add(new Claim(uc.Type, uc.Value));
            }

            // remove name claim that matches username if it exists
            var username = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name && x.Value == user.Username);
            if (claims.Count(x => x.Type == JwtClaimTypes.Name) > 1 && username != null) {
                claims.Remove(username);
            }

            // make sure there is a name claim
            if (!user.UserClaims.Any(x => x.Type == JwtClaimTypes.Name)) {
                claims.Add(new Claim(JwtClaimTypes.Name, user.Username));
            }

            // remove name claim that matches username if it exists
            var ipaddress = claims.FirstOrDefault(x => x.Type == "ip_address");
            if (ipaddress != null) {
                claims.Remove(ipaddress);
            }
            claims.Add(new Claim("ip_address", user.LastLoginIPAddress));

            // TODO: filter claims to match requested claims
            if (requestedClaims != null) {
                //TODO: If there were requested claims, we would add them here specifically and not just add all
            }

            return claims;
        }
    }
}
