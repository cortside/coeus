using System;
using System.Text.RegularExpressions;
using Acme.ShoppingCart.CatalogApi.Models.Responses;
using Cortside.MockServer;
using Cortside.MockServer.Builder;
using Cortside.RestApiClient.Authenticators.OpenIDConnect;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Acme.ShoppingCart.CatalogApi.Tests.Mock {
    public class CatalogMock : IMockHttpMock {
        public void Configure(MockHttpServer server) {
            var getUserUrl = new Regex(@"^\/api/v1/items\/([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$");
            server.WireMockServer
               .Given(
                   Request.Create().WithPath(p => getUserUrl.IsMatch(p)).UsingGet()
               )
               .RespondWith(
                   Response.Create()
                       .WithStatusCode(200)
                       .WithHeader("Content-Type", "application/json")
                       .WithBody(r => JsonConvert.SerializeObject(
                           new CatalogItem() {
                               ItemId = Guid.NewGuid(),
                               Name = "Foo",
                               Sku = r.PathSegments[3],
                               UnitPrice = 12.34M
                           }
                       ))
               );
            server.WireMockServer
                .Given(
                    Request.Create().WithPath("/connect/token").UsingPost()
                )
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_ => JsonConvert.SerializeObject(
                            new TokenResponse {
                                AccessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjY1QjBCQTk2MUI0NDMwQUJDNTNCRUI5NkVDMjBDNzQ5OTdGMDMwMzJSUzI1NiIsInR5cCI6IkpXVCIsIng1dCI6IlpiQzZsaHRFTUt2Rk8tdVc3Q0RIU1pmd01ESSJ9.eyJuYmYiOjE2NDI3OTE3ODMsImV4cCI6MTY0MjgwOTc4MywiaXNzIjoiaHR0cHM6Ly9pZGVudGl0eXNlcnZlci5rOHMuZW5lcmJhbmsuY29tIiwiYXVkIjpbInRlcm1zY2FsY3VsYXRvci1hcGkiLCJodHRwczovL2lkZW50aXR5c2VydmVyLms4cy5lbmVyYmFuay5jb20vcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6ImF1dG9tYXRpb24uc3lzdGVtMSIsInN1YiI6IjQ1OWI5NGJhLTEwMTUtNDJhNC05YzRkLTVjNThiZmZhM2E5YSIsImdyb3VwcyI6ImVjYmRhM2Y3LWQ2MjgtNGU1YS04NzljLWNmNmNhYWQyM2JhMSIsInN1YmplY3RfdHlwZSI6ImNsaWVudCIsImlhdCI6MTY0Mjc5MTc4Mywic2NvcGUiOlsidGVybXNjYWxjdWxhdG9yLWFwaSJdfQ.siCAaMAp6O9ZiWGM7c7b_U3gRkx-lb4IqfxxFyI5LBLPGB9bSHy6fl5PHaIIjs_VO0TnuVv7gjafqUTkVEbpJbf3pbQOdvzNzfGKTWgEn44dbiz0ROuDy2_qpGqAUlo1r5nkYcuDUtyLP5FsrmxSjUP0DlanuWNWSiqx5YVdzenGeLSFD59cCszZnmS2Q9KPV5MOkaUEJnd2D44UJIcccoEdRhSDkY8a6Fs03Iodf2bzvcb2mZ6aiRHhjTo2XvqiG0azLIX2W735eiSX52qoUB6ae6bDzGpEXeE3z3bRw5fomgy40XXyct64IRnIW_Hfk0ZmNyw1L51ZJwlYSF7OPA",
                                ExpiresIn = 300,
                                TokenType = "Bearer"
                            }
                        ))
                    );
        }
    }
}
