using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Acme.ShoppingCart.WebApi.IntegrationTests.Data.Ids;
using Acme.ShoppingCart.WebApi.IntegrationTests.Helpers.Mocks.Models;
using Newtonsoft.Json;
using Serilog;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Helpers.Mocks {
    public class SubjectMock : IWireMockBuilder {
        public void Configure(WireMockServer server) {


            // todo: add support for feature flags  -- http://bitbucket:7990/projects/OA/repos/onlineapplication/pull-requests/4798/diff#src/EBOA.ApplicationApi.IntegrationTests/Helpers/Mocks/BaseWireMock.cs


            var subjects = JsonConvert.DeserializeObject<Subjects>(File.ReadAllText(@"./Helpers/Mocks/subjects.json"));
            foreach (var subject in subjects.SubjectsList) {
                Log.Logger.Debug($"Client: {subject.ClientId}");
                var claims = new List<SubjectClaim>();
                claims.AddRange(subject.Claims);
                claims.Add(new SubjectClaim { Type = "sub", Value = subject.SubjectId });
                claims.Add(new SubjectClaim { Type = "upn", Value = subject.ClientId });

                var dictClaims = claims.ToDictionary(x => x.Type, x => x.Value);
                var claimsJson = JsonConvert.SerializeObject(dictClaims);

                foreach (var policy in subject.Policies) {
                    server
                        .Given(
                        Request.Create().WithPath($"/runtime/policy/{policy.PolicyName}")
                        .WithBody(b => b != null && b.Contains(subject.SubjectId))
                            .UsingPost()
                        )
                        .RespondWith(
                            Response.Create()
                                .WithStatusCode(200)
                                .WithBody(r => JsonConvert.SerializeObject(policy.Authorization))
                        );
                }

                server
                    .Given(
                        Request.Create().WithPath("/connect/token")
                            .WithBody(b => b != null && b.Contains(subject.ClientId))
                            .UsingPost()
                    )
                    .RespondWith(
                        Response.Create()
                            .WithStatusCode(200)
                            .WithHeader("Content-Type", "application/json")
                            .WithBody(r => JsonConvert.SerializeObject(new AuthenticationResponseModel {
                                TokenType = "Bearer",
                                ExpiresIn = "3600",
                                AccessToken = subject.ReferenceToken
                            }))
                );

                server
                    .Given(
                        Request.Create().WithPath($"/connect/introspect")
                            .WithBody(b => b != null && b.Contains(subject.ReferenceToken))
                            .UsingPost()
                    )
                    .RespondWith(
                        Response.Create()
                            .WithStatusCode(200)
                            .WithBody(r => claimsJson)
                    );
            }
        }
    }
}
