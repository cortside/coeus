using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Cortside.IdentityServer.Data;
using Cortside.IdentityServer.WebApi.Models.Input;

namespace Cortside.IdentityServer.WebApi.Services {
    public interface IUserService {

        User FindBySubjectId(Guid subjectId);
        void UpdatePassword(Guid subjectId, UpdatePasswordModel model);
        User FindByExternalProvider(string provider, string providerSubjectId);
        Task<User> AutoProvisionUser(string provider, string userId, List<Claim> claims);
        Task<User> CreateUser(CreateUserModel userCreateRequest);
        Task<User> UpdateUser(Guid userId, UpdateUserModel userUpdateRequest);
        User UpdateUserLock(Guid subjectId, UpdateLockModel model);
        void DeactivateUser(Guid userId);
        Task<User> UpdateExternalUserClaims(string provider, string userId, List<Claim> claims);
    }
}
