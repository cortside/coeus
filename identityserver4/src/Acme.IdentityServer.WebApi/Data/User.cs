using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using Acme.IdentityServer.WebApi.Exceptions;
using Acme.IdentityServer.WebApi.Models;
using Acme.IdentityServer.WebApi.Models.Enumerations;
using IdentityModel;

namespace Acme.IdentityServer.WebApi.Data {
    public class User : AuditableEntity {
        public User() {
            UserClaims = new List<UserClaim>();
            UserRoles = new List<UserRole>();
            LoginAttempts = new List<LoginAttempt>();
        }
        public Guid UserId { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
        public string Salt { set; get; }
        public string UserStatus { set; get; } //TODO: Want to map this to an enum some how. Need to look into this.
        public DateTime EffectiveDate { set; get; }
        public DateTime? ExpirationDate { set; get; }
        public int LoginCount { set; get; }
        public DateTime? LastLogin { set; get; }
        public string LastLoginIPAddress { set; get; }
        public string LockedReason { get; set; }
        public DateTime TermsOfUseAcceptanceDate { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } //This should be Roles, not UserRoles, but EFCore cannot handle this right now.
        public string ProviderName { get; internal set; }
        public string ProviderSubjectId { get; internal set; }
        public bool IsLocked { set; get; }
        public string TwoFactor { get; internal set; }
        public bool TwoFactorVerified { get; set; }
        public ICollection<UserClaim> UserClaims { get; set; }
        public ICollection<LoginAttempt> LoginAttempts { get; set; }

        public void Update(string username, UserStatus? status, List<UserClaimModel> claimModels, List<string> restrictedEmailDomains) {
            if (string.IsNullOrWhiteSpace(username)) {
                throw new InvalidValueMessage(nameof(Username), username);
            }

            List<UserClaim> filtered = new List<UserClaim>();
            foreach (var model in claimModels) {
                if (model.Type == JwtClaimTypes.Subject || model.Type == "upn") {
                    throw new InvalidValueMessage(nameof(model.Type), model.Type);
                }
                if (restrictedEmailDomains != null && model.Type.ToLower() == "email") {
                    var email = new MailAddress(model.Value);
                    if (restrictedEmailDomains.Contains(email.Host)) {
                        throw new InvalidEmailMessage();
                    }
                }
                filtered.Add(new UserClaim() {
                    Type = model.Type,
                    Value = model.Value
                });
            }

            // remove name claim that matches username if it exists
            var usernameClaim = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.Name && x.Value == username);
            if (filtered.Count(x => x.Type == JwtClaimTypes.Name) > 1 && usernameClaim != null) {
                filtered.Remove(usernameClaim);
            }

            // remove claims for provider and not in the users claims
            var claimsToRemove = new List<UserClaim>();
            foreach (var claim in this.UserClaims) {
                if (!filtered.Any(c => c.Type == claim.Type && c.Value == claim.Value)) {
                    claimsToRemove.Add(claim);
                }
            }
            var unc = this.UserClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name && x.Value == username);
            if (this.UserClaims.Count(x => x.Type == JwtClaimTypes.Name) > 1 && unc != null) {
                claimsToRemove.Add(unc);
            }
            claimsToRemove.ForEach(c => this.UserClaims.Remove(c));

            // add claims that don't already exist for the provider in the users claims
            foreach (var c in filtered) {
                if (!this.UserClaims.Any(uc => uc.Type == c.Type && uc.Value == c.Value)) {
                    this.UserClaims.Add(c);
                }
            }

            // update username
            this.Username = username;
            if (status.HasValue) {
                this.UserStatus = status.ToString();
            }
        }

        public void UpdateExternal(List<Claim> claims) {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();
            Claim upnClaim = null;

            foreach (var claim in claims) {
                // try translate type
                string translatedClaimType = claim.Type;
                if (claim.Type == ClaimTypes.Name) {
                    translatedClaimType = JwtClaimTypes.Name;
                } else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type)) {
                    translatedClaimType = JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type];
                }

                // reject certain claim types
                if (translatedClaimType == "upn") {
                    upnClaim = claim;
                    continue;
                }
                if (translatedClaimType == JwtClaimTypes.Subject) {
                    continue;
                }

                filtered.Add(new Claim(translatedClaimType, claim.Value));
            }

            // update username
            // check if a 'upn' (User Principal Name) claim is available otherwise check if email is available, otherwise fallback to subject id
            this.Username = upnClaim?.Value ?? filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Email)?.Value ?? this.UserId.ToString();

            // if no display name was provided, try to construct by first and/or last name
            if (!filtered.Any(x => x.Type == JwtClaimTypes.Name)) {
                var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
                if (first != null && last != null) {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                } else if (first != null) {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                } else if (last != null) {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // remove claims for provider and not in the users claims
            var claimsToRemove = new List<UserClaim>();
            foreach (var claim in this.UserClaims.Where(uc => uc.ProviderName == this.ProviderName)) {
                if (!filtered.Any(c => c.Type == claim.Type && c.Value == claim.Value)) {
                    claimsToRemove.Add(claim);
                }
            }

            claimsToRemove.ForEach(c => this.UserClaims.Remove(c));

            // add claims that don't already exist for the provider in the users claims
            foreach (var c in filtered) {
                if (!this.UserClaims.Any(uc => uc.Type == c.Type && uc.Value == c.Value && uc.ProviderName == this.ProviderName)) {
                    var claim = new UserClaim() {
                        UserId = this.UserId,
                        ProviderName = this.ProviderName,
                        Type = c.Type,
                        Value = c.Value
                    };
                    this.UserClaims.Add(claim);
                }
            }
        }
    }
}
