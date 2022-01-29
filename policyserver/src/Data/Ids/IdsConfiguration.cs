using System.Collections.Generic;

namespace Acme.WebApiStarter.WebApi.IntegrationTests.Data.Ids {
    public class IdsConfiguration {
        public string Issuer { get; set; }
        public string Jwks_uri { get; set; }
        public string Authorization_endpoint { get; set; }
        public string Token_endpoint { get; set; }
        public string Userinfo_endpoint { get; set; }
        public string End_session_endpoint { get; set; }
        public string Check_session_iframe { get; set; }
        public string Revocation_endpoint { get; set; }
        public string Introspection_endpoint { get; set; }
        public string Device_authorization_endpoint { get; set; }
        public bool Frontchannel_logout_supported { get; set; }
        public bool Frontchannel_logout_session_supported { get; set; }
        public bool Backchannel_logout_supported { get; set; }
        public bool Backchannel_logout_session_supported { get; set; }
        public List<string> Scopes_supported { get; set; }
        public List<string> Claims_supported { get; set; }
        public List<string> Grant_types_supported { get; set; }
        public List<string> Response_types_supported { get; set; }
        public List<string> Response_modes_supported { get; set; }
        public List<string> Token_endpoint_auth_methods_supported { get; set; }
        public List<string> Subject_types_supported { get; set; }
        public List<string> Id_token_signing_alg_values_supported { get; set; }
        public List<string> Code_challenge_methods_supported { get; set; }
    }
}
