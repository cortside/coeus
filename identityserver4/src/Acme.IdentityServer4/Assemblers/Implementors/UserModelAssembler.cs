using Cortside.IdentityServer.Data;
using Cortside.IdentityServer.WebApi.Data;

namespace Cortside.IdentityServer.WebApi.Assemblers.Implementors {
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
