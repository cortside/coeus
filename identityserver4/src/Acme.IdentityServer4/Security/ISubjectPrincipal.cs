using System;
using System.Security.Principal;

namespace Cortside.IdentityServer.WebApi.Security {
    public interface ISubjectPrincipal : IPrincipal {
        Guid SubjectId { get; }
        bool IsAuthenticated { get; }
    }
}
