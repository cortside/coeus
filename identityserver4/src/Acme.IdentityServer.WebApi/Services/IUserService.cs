using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Acme.IdentityServer.WebApi.Data;
using Acme.IdentityServer.WebApi.Models.Input;

namespace Acme.IdentityServer.WebApi.Services {
    public interface IUserService {

        Task<User> FindBySubjectIdAsync(Guid subjectId);
        Task UpdatePassword(Guid subjectId, UpdatePasswordModel model);
        User FindByExternalProvider(string provider, string providerSubjectId);
        Task<User> AutoProvisionUser(string provider, string userId, List<Claim> claims);
        Task<User> CreateUser(CreateUserModel userCreateRequest);
        Task<User> UpdateUser(Guid userId, UpdateUserModel userUpdateRequest);
        Task<User> UpdateUserLock(Guid subjectId, UpdateLockModel model);
        string GetTwoFactorURI(Guid subjectId);
        Task<bool> VerifyCurrentTOTP(Guid subjectId, string code);
        Task<string> GenerateAndSetNewTOTPCode(Guid subjectId);
        Task DeactivateUser(Guid userId);
        Task<User> UpdateExternalUserClaims(string provider, string userId, List<Claim> claims);
    }
}
