using System;
using System.Security.Principal;

namespace Acme.IdentityServer.WebApi.Security {
    public interface ISubjectPrincipal : IPrincipal {
        Guid SubjectId { get; }
        bool IsAuthenticated { get; }
    }
}
