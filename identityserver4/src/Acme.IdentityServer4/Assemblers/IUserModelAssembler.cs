using Cortside.IdentityServer.Data;
using Cortside.IdentityServer.WebApi.Data;

namespace Cortside.IdentityServer.WebApi.Assemblers {
    public interface IUserModelAssembler {
        Models.Output.UserOutputModel ToUserOutputModel(User user);
        Models.Output.UserOutputModel ToUserOutputModel(Client client);
    }
}
