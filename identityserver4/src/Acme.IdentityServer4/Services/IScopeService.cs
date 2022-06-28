using System.Collections.Generic;

namespace Cortside.IdentityServer.WebApi.Services {
    public interface IScopeService {

        List<string> GetAll();
    }
}
