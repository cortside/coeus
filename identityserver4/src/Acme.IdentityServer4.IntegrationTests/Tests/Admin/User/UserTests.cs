using Cortside.IdentityServer.WebApi.IntegrationTests;
using Cortside.IdentityServer.WebApi.IntegrationTests.Extensions;
using Cortside.IdentityServer.WebApi.Models;
using FluentAssertions;
using IdentityModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cortside.IdentityServer.WebApi.IntegrationTests.Tests.User {

    public class UserTest : IClassFixture<IntegrationTestFixture> {
        private readonly IntegrationTestFixture integrationTestFixture;
        private static readonly Random random = new Random();

        public UserTest(IntegrationTestFixture integrationTestFixture) {
            this.integrationTestFixture = integrationTestFixture;
        }

        [Fact]
        public async Task ShouldGetUser() {
            integrationTestFixture.Client.DefaultRequestHeaders.Authorization = await GetTokenHeader();
            string userId = integrationTestFixture.DefaultUserId.ToString();
            var getUserResponse = await integrationTestFixture.Client.GetAsync($"/api/Users/{userId}");
            getUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            dynamic responseData = JObject.Parse(getUserResponse.Content.ReadAsStringAsync().Result);
            string username = responseData.username;
            string userIdResp = responseData.userId;
            string providerName = responseData.providerName;
            username.Should().Be(integrationTestFixture.UserName);
            userIdResp.Should().Be(userId.ToLower());
            providerName.Should().BeNull();
        }

        [Fact]
        public async Task ShouldCreateUser() {
            // arrange
            var createUserRequestBody = getUserRequestBody();

            // act
            var response = await CreateUser(user: createUserRequestBody);

            // assert
            response.UserStatus.Should().Be(Models.Enumerations.UserStatus.Active);
        }

        [Fact]
        public async Task ShouldUpdateUser() {
            // create user
            var createUserRequestBody = getUserRequestBody();
            var createUserRespData = await CreateUser(user: createUserRequestBody);

            // update user
            var updateModel = new Models.Input.UpdateUserModel() {
                Username = random.RandomString(),
                Status = Models.Enumerations.UserStatus.Active,
                Claims = new List<UserClaimModel> {
                    new UserClaimModel() {
                        Type = ClaimTypes.Gender,
                        Value = "male"
                    },
                    new UserClaimModel() {
                        Type = JwtClaimTypes.PhoneNumber,
                        Value = "8012223434"
                    },
                    new UserClaimModel()
                    {
                        Type = "firstname",
                        Value = "jane"
                    }
                }
            };
            string putBodyString = JsonConvert.SerializeObject(updateModel,
                new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });
            StringContent putRequestBody = new StringContent(putBodyString, Encoding.UTF8, "application/json");
            var updateUserResp = await integrationTestFixture.Client.PutAsync("/api/Users/" + createUserRespData.UserId.ToString(), putRequestBody);
            updateUserResp.Headers.FirstOrDefault(h => h.Key == "Location'").Should().NotBeNull();
            var updateUserRespData = JsonConvert.DeserializeObject<Models.Output.UserOutputModel>(updateUserResp.Content.ReadAsStringAsync().Result);
            updateUserResp.StatusCode.Should().Be(HttpStatusCode.OK);
            updateUserRespData.Username.Should().Be(updateModel.Username);
            updateUserRespData.UserId.Should().Be(createUserRespData.UserId);
            updateUserRespData.UserStatus.Should().Be(Models.Enumerations.UserStatus.Active);
            updateUserRespData.Claims.Count.Should().Be(updateModel.Claims.Count);
            foreach (var c in updateModel.Claims) {
                updateUserRespData.Claims.Should().Contain(x => x.Type == c.Type && x.Value == c.Value);
            }
            updateUserRespData.IsActive.Should().BeTrue();
            updateUserRespData.LastModifiedBySubjectId.Should().Be(integrationTestFixture.CreateUserId);

            // update password
            var passwordModel = new Models.Input.UpdatePasswordModel() {
                Password = "lessStuff"
            };
            StringContent updatePasswordRequestContent = new StringContent(JsonConvert.SerializeObject(passwordModel), Encoding.UTF8, "application/json");
            var updatePasswordResponse = await integrationTestFixture.Client.PutAsync($"/api/users/{createUserRespData.UserId:d}/password", updatePasswordRequestContent);
            updatePasswordResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task ShouldDeleteUser() {
            var createUserRequestBody = getUserRequestBody();
            var createUserRespData = await CreateUser(user: createUserRequestBody);
            var request = new HttpRequestMessage {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"http://localhost/api/Users/{createUserRespData.UserId}"),
                Content = new StringContent(JsonConvert.SerializeObject(createUserRespData.LastModifiedBySubjectId), Encoding.UTF8, "application/json")
            };
            var deleteUserResp = await integrationTestFixture.Client.SendAsync(request);

            deleteUserResp.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        private async Task<AuthenticationHeaderValue> GetTokenHeader() {
            Dictionary<string, string> authBodyDict = new Dictionary<string, string>()
            {
                { "grant_type", "client_credentials" },
                { "client_id", integrationTestFixture.ClientName },
                { "client_secret", integrationTestFixture.ClientSecret },
                { "scope", integrationTestFixture.Scope },
            };
            integrationTestFixture.Client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            var getTokenResponse = await integrationTestFixture.Client.PostAsync($"/connect/token", new FormUrlEncodedContent(authBodyDict));
            var t = await getTokenResponse.Content.ReadAsStringAsync();
            dynamic tokenJson = JObject.Parse(await getTokenResponse.Content.ReadAsStringAsync());
            string token = tokenJson.access_token;
            integrationTestFixture.Client.DefaultRequestHeaders.Clear();
            return new AuthenticationHeaderValue("Bearer", token);
        }

        private Models.Input.CreateUserModel getUserRequestBody() {
            return new Models.Input.CreateUserModel() {
                Username = random.RandomString(),
                Password = integrationTestFixture.Password,
                Claims = new List<UserClaimModel>
                {
                    new UserClaimModel()
                    {
                        Type="yousay",
                        Value = "potato"
                    },
                    new UserClaimModel()
                    {
                        Type = ClaimTypes.Gender,
                        Value = "female"
                    }
                },
            };
        }

        private async Task<Models.Output.UserOutputModel> CreateUser(Models.Input.CreateUserModel user) {
            integrationTestFixture.Client.DefaultRequestHeaders.Authorization = await GetTokenHeader();
            integrationTestFixture.Client.DefaultRequestHeaders.Add("Accept", "application/json");
            string bodyString = JsonConvert.SerializeObject(user,
                new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });
            var requestBody = new StringContent(bodyString, Encoding.UTF8, "application/json");
            var createUserResp = await integrationTestFixture.Client.PostAsync("/api/Users", requestBody);
            createUserResp.Headers.FirstOrDefault(h => h.Key == "Location").Should().NotBeNull();
            var createUserRespData = JsonConvert.DeserializeObject<Models.Output.UserOutputModel>(createUserResp.Content.ReadAsStringAsync().Result);
            createUserResp.StatusCode.Should().Be(HttpStatusCode.Created);
            createUserRespData.LastModifiedBySubjectId.Should().Be(integrationTestFixture.CreateUserId);
            createUserRespData.Claims.Count.Should().Be(2);
            createUserRespData.Claims.Should().Contain(c => c.Type == user.Claims.First().Type);
            createUserRespData.Claims.Should().Contain(c => c.Value == user.Claims.First().Value);
            createUserRespData.Username.Should().Be(user.Username);
            createUserRespData.UserId.Should().NotBe(Guid.Parse("00000000-0000-0000-0000-000000000000"));
            createUserRespData.UserId.Should().NotBe(integrationTestFixture.CreateUserId);
            createUserRespData.IsActive.Should().BeTrue();
            return createUserRespData;
        }
    }
}
