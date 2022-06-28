using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cortside.IdentityServer.Data;
using Cortside.IdentityServer.WebApi.Models;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.IdentityServer.Services {

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

            if (!claims.Any(x => x.Type == "sub")) {
                claims.Add(new Claim("sub", subjectId));
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
            Action<string, string> addClaim = (type, value) => {
                claims.Add(new Claim(type, value));
            };

            // make sure username is the upn claim
            addClaim("upn", user.Username);

            foreach (var uc in user.UserClaims ?? new List<UserClaim>()) {
                addClaim(uc.Type, uc.Value);
            }

            // remove name claim that matches username if it exists
            var username = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name && x.Value == user.Username);
            if (claims.Count(x => x.Type == JwtClaimTypes.Name) > 1 && username != null) {
                claims.Remove(username);
            }

            // make sure there is a name claim
            if (!user.UserClaims.Any(x => x.Type == JwtClaimTypes.Name)) {
                addClaim(JwtClaimTypes.Name, user.Username);
            }

            // TODO: filter claims to match requested claims
            if (requestedClaims != null) {
                //TODO: If there were requested claims, we would add them here specifically and not just add all
            }

            return claims;
        }
    }
}
