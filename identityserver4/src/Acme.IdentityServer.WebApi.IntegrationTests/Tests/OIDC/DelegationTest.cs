using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Acme.IdentityServer.WebApi.IntegrationTests.Tests.OIDC {

    public class DelegationTest : IClassFixture<IntegrationTestFixture> {
        private readonly IntegrationTestFixture integrationTestFixture;

        public DelegationTest(IntegrationTestFixture integrationTestFixture) {
            this.integrationTestFixture = integrationTestFixture;
        }

        [Fact]
        public async Task ShouldHaveSubjectTypeClaim() {
            // act
            var encodedToken = await GetToken("acme", "eboa.webapi", null);

            // assert
            Assert.NotNull(encodedToken);

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(encodedToken);

            var subjectType = jwtSecurityToken.Claims.First(x => x.Type == "subject_type").Value;
            Assert.Equal("client", subjectType);

            var email = jwtSecurityToken.Claims.First(x => x.Type == "email").Value;
            Assert.Equal("wile_e_coyote@acme.com", email);
        }

        [Fact]
        public async Task ShouldHaveDelegationGrantTypeClaim() {
            // act
            var encodedToken = await GetToken("acme", "eboa.webapi", null);

            // assert
            Assert.NotNull(encodedToken);

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(encodedToken);

            var any = jwtSecurityToken.Claims.Any(x => x.Type == "grant_type" && x.Value == "delegation");
            Assert.True(any);
        }

        [Fact]
        public async Task ShouldDelegate() {
            // arrange
            var serviceToken = await GetToken("application-api", "document-api", null);

            // act
            var delegateToken = await GetToken("acme", "eboa.webapi", serviceToken);

            // assert
            Assert.NotNull(delegateToken);

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(delegateToken);

            var sub = jwtSecurityToken.Claims.First(x => x.Type == "sub").Value;
            var act = jwtSecurityToken.Claims.First(x => x.Type == "act").Value;

            Assert.Equal("36c90fc9-496c-40cd-87d9-23dc3f79ae2c", sub);
            Assert.Equal("d2e9c5de-f619-4a64-b657-32f8abc5ebb6", act);
        }

        [Fact]
        public async Task ShouldDelegateOriginalActorOnMultipleDelegations() {
            // arrange
            var serviceToken = await GetToken("application-api", "document-api", null);
            var delatateToken1 = await GetToken("acme", "eboa.webapi", serviceToken);

            // act
            var delegateToken2 = await GetToken("roadrunnercorp", "eboa.webapi", delatateToken1);

            // assert
            Assert.NotNull(delegateToken2);

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(delegateToken2);

            var sub = jwtSecurityToken.Claims.First(x => x.Type == "sub").Value;
            var act = jwtSecurityToken.Claims.First(x => x.Type == "act").Value;

            var client_id = jwtSecurityToken.Claims.First(x => x.Type == "client_id").Value;
            var act_client_id = jwtSecurityToken.Claims.First(x => x.Type == "act_client_id").Value;

            Assert.Equal("7fd2e357-f41d-4e41-95db-85d7b4f8518e", sub);
            Assert.Equal("d2e9c5de-f619-4a64-b657-32f8abc5ebb6", act);

            Assert.Equal("roadrunnercorp", client_id);
            Assert.Equal("application-api", act_client_id);

            Assert.False(jwtSecurityToken.Claims.Any(x => x.Type.StartsWith("act_act_")));
        }

        private async Task<string> GetToken(string clientId, string scope, string tokenToDelegate) {
            Dictionary<string, string> authBodyDict = new Dictionary<string, string>()
            {
                { "grant_type", tokenToDelegate == null ? "client_credentials" : "delegation" },
                { "scope", scope },
                { "client_id", clientId },
                { "client_secret", integrationTestFixture.ClientSecret },
                { "token", tokenToDelegate }
            };
            integrationTestFixture.Client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            var getTokenResponse = await integrationTestFixture.Client.PostAsync($"/connect/token", new FormUrlEncodedContent(authBodyDict));
            var content = await getTokenResponse.Content.ReadAsStringAsync();
            dynamic tokenJson = JObject.Parse(content);
            string token = tokenJson.access_token;
            integrationTestFixture.Client.DefaultRequestHeaders.Clear();
            return token;
        }
    }
}
