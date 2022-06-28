using System.Collections.Generic;

namespace EnerBank.IdentityServer.WebApi.Services {
    public interface IScopeService {

        List<string> GetAll();
    }
}
