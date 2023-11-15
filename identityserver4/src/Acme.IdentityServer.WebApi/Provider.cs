using System.Collections.Generic;

namespace Acme.IdentityServer.WebApi {
    public class Provider {
        public string Key { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string CallbackPath { get; set; }
        public List<string> Scopes { get; set; }
        public string ProviderIdClaim { get; set; }
        public string Scheme { get; set; }
        public string UsernameClaim { get; set; }
        public List<string> Claims { get; set; }
        public string DisplayName { get; set; }
        public bool AddIdTokenHint { get; set; }
    }
}
