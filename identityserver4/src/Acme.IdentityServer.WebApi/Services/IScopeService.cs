using System.Collections.Generic;

namespace Acme.IdentityServer.WebApi.Services {
    public interface IScopeService {

        List<string> GetAll();
    }
}
