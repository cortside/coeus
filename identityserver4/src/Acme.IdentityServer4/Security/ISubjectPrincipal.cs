using System;
using System.Security.Principal;

namespace EnerBank.IdentityServer.WebApi.Security {
    public interface ISubjectPrincipal : IPrincipal {
        Guid SubjectId { get; }
        bool IsAuthenticated { get; }
    }
}
