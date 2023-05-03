using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Acme.IdentityServer.WebApi.Security {
    public class SubjectPrincipal : ClaimsPrincipal, ISubjectPrincipal {
        public Guid SubjectId {
            get {
                return Guid.TryParse(Claims?.Where(c => c.Type == "sub")?.Select(c => c.Value)?.FirstOrDefault(), out Guid result)
                    ? result
                    : Guid.Empty;
            }
        }
        public bool IsAuthenticated => Identity.IsAuthenticated;
        public SubjectPrincipal(IPrincipal principal) : base(principal) {

        }

        public SubjectPrincipal() : base() { }
    }
}
