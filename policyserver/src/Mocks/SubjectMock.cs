using Newtonsoft.Json;
using PolicyServer.Data.Ids;
using PolicyServer.Mocks.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace PolicyServer.Mocks
{
    public class SubjectMock : IWireMockBuilder
    {
        public void Configure(WireMockServer server)
        {
            var subjects = JsonConvert.DeserializeObject<Subjects>(File.ReadAllText("./subjects.json"));
            foreach (var subject in subjects.SubjectsList)
            {
                Log.Logger.Debug("Client: {ClientId}", subject.ClientId);
                var claims = new List<SubjectClaim>();
                claims.AddRange(subject.Claims);
                claims.Add(new SubjectClaim { Type = "sub", Value = subject.SubjectId });
                claims.Add(new SubjectClaim { Type = "upn", Value = subject.ClientId });
                var dictClaims = claims.ToDictionary(x => x.Type, x => x.Value);
                var claimsJson = JsonConvert.SerializeObject(dictClaims);

                foreach (var policy in subject.Policies)
                {
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
                            .WithBody(r => JsonConvert.SerializeObject(new AuthenticationResponseModel
                            {
                                TokenType = "Bearer",
                                ExpiresIn = "3600",
                                AccessToken = subject.ReferenceToken
                            }))
                );
                server
                    .Given(
                        Request.Create().WithPath("/connect/token")
                            .WithHeader(h => h.ContainsKey("Authorization") && h["Authorization"]?.FirstOrDefault() != null && h["Authorization"]?.FirstOrDefault().Replace("Basic ", "").DecodeBase64().Contains(subject.ClientId) == true)
                            .UsingPost()
                    )
                    .RespondWith(
                        Response.Create()
                            .WithStatusCode(200)
                            .WithHeader("Content-Type", "application/json")
                            .WithBody(r => JsonConvert.SerializeObject(new AuthenticationResponseModel
                            {
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
