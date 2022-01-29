using System.IO;
using System.Linq;
using Acme.ShoppingCart.WebApi.IntegrationTests.Helpers.Mocks.Models;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Helpers.Mocks {
    public class SubjectMock : IWireMockBuilder {
        public void Configure(WireMockServer server) {
            var subjects = JsonConvert.DeserializeObject<Subjects>(File.ReadAllText(@"./Helpers/Mocks/subjects.json"));
            foreach (var subject in subjects.SubjectsList) {
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
                        Request.Create().WithPath($"/connect/introspect")
                            .WithBody(b => b != null && b.Contains(subject.ReferenceToken))
                            .UsingPost()
                    )
                    .RespondWith(
                        Response.Create()
                            .WithStatusCode(200)
                            .WithBody(r => $"{{\"active\" : true, \"sub\": \"{subject.SubjectId}\"}}")
                    );
            }
        }
    }
}
