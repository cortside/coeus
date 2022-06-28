using EnerBank.IdentityServer.Data;
using EnerBank.IdentityServer.WebApi.Data;

namespace EnerBank.IdentityServer.WebApi.Assemblers {
    public interface IUserModelAssembler {
        Models.Output.UserOutputModel ToUserOutputModel(User user);
        Models.Output.UserOutputModel ToUserOutputModel(Client client);
    }
}
