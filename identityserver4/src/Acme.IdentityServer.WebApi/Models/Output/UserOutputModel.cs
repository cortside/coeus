using System;
using System.Collections.Generic;
using Acme.IdentityServer.WebApi.Models.Enumerations;

namespace Acme.IdentityServer.WebApi.Models.Output {
    public class UserOutputModel {
        /// <summary>
        /// Identity Server User Identification
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The user identifier from the Provider that is making the request.
        /// </summary>
        public Guid LastModifiedBySubjectId { get; set; }

        public string Username { get; set; }

        /// <summary>
        /// User Status
        /// </summary>
        public UserStatus UserStatus { set; get; }

        public bool IsActive { get; set; }

        /// <summary>
        /// A User Claim is a key to value pair and can represent any data that needs to be associated with a user.
        /// Identity Server does not use User Claims for any decisions. 
        /// It will simply pass the User Claims on request/configuration.
        /// </summary>
        public List<UserClaimModel> Claims { get; set; }

        /// <summary>
        /// (Optional) Provider name like AAD (Azure Active Directory)
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// (Optional) Provider's user identify
        /// </summary>
        public string ProviderSubjectId { get; set; }

        /// <summary>
        /// A User's locked login status 
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// The reason the user was last locked
        /// </summary>
        public string LockedReason { get; set; }

        /// <summary>
        /// Last login date
        /// </summary>
        public DateTime? LastLogin { set; get; }
    }
}
