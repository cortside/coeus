using Newtonsoft.Json;

namespace Acme.WebApiStarter.WebApi.IntegrationTests.Data.Ids {
    public class AuthenticationResponseModel {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
