using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models.Input;

namespace Acme.IdentityServer.WebApi.Services {
    public interface IUserService {

        Task<User> FindBySubjectIdAsync(Guid subjectId);
        void UpdatePassword(Guid subjectId, UpdatePasswordModel model);
        User FindByExternalProvider(string provider, string providerSubjectId);
        Task<User> AutoProvisionUser(string provider, string userId, List<Claim> claims);
        Task<User> CreateUser(CreateUserModel userCreateRequest);
        Task<User> UpdateUser(Guid userId, UpdateUserModel userUpdateRequest);
        User UpdateUserLock(Guid subjectId, UpdateLockModel model);
        string GetTwoFactorURI(Guid subjectId);
        bool VerifyCurrentTOTP(Guid subjectId, string code);
        string GenerateAndSetNewTOTPCode(Guid subjectId);
        void DeactivateUser(Guid userId);
        Task<User> UpdateExternalUserClaims(string provider, string userId, List<Claim> claims);
    }
}
