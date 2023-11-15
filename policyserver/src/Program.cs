using System;
using System.Threading.Tasks;
using Cortside.AspNetCore;
using Cortside.MockServer;
using Cortside.MockServer.AccessControl;
using Cortside.MockServer.Mocks;
using Newtonsoft.Json;
using PolicyServer.Mocks;

namespace PolicyServer {
    public static class Program {
        public static Task Main(string[] args) {
            // setup global default json serializer settings
            JsonConvert.DefaultSettings = JsonNetUtility.GlobalDefaultSettings;

            var server = MockHttpServer.CreateBuilder(Guid.NewGuid().ToString(), 5001)
                .AddMock<CommonMock>()
                .AddMock(new IdentityServerMock("./Data/discovery.json", "./Data/jwks.json"))
                .AddMock(new SubjectMock("./Data/subjects.json"))
                .AddMock(new CatalogMock("./Data/items.json"))
                .Build();

            return server.WaitForCancelKeyPressAsync();
        }
    }
}
