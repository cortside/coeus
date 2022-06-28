using EnerBank.IdentityServer.Data;
using EnerBank.IdentityServer.WebApi.Data;

namespace EnerBank.IdentityServer.WebApi.Assemblers.Implementors {
    public class UserModelAssembler : BaseModelAssembler, IUserModelAssembler {
        public UserModelAssembler() : base() { }

        public Models.Output.UserOutputModel ToUserOutputModel(User user) {
            return mapper.Map<Models.Output.UserOutputModel>(user);
        }

        public Models.Output.UserOutputModel ToUserOutputModel(Client client) {
            return mapper.Map<Models.Output.UserOutputModel>(client);
        }
    }
}
