using System.Collections.Generic;

namespace Acme.WebApiStarter.WebApi.IntegrationTests.Data.Ids {
    public class Key {
        public string Kty { get; set; }
        public string Use { get; set; }
        public string Kid { get; set; }
        public string X5t { get; set; }
        public string E { get; set; }
        public string N { get; set; }
        public List<string> X5c { get; set; }
        public string Alg { get; set; }
    }

    public class IdsJwks {
        public List<Key> Keys { get; set; }
    }
}
