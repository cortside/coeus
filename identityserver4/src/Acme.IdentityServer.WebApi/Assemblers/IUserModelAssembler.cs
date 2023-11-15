using Acme.IdentityServer.Data;
using Acme.IdentityServer.WebApi.Data;

namespace Acme.IdentityServer.WebApi.Assemblers {
    public interface IUserModelAssembler {
        Models.Output.UserOutputModel ToUserOutputModel(User user);
        Models.Output.UserOutputModel ToUserOutputModel(Client client);
    }
}
