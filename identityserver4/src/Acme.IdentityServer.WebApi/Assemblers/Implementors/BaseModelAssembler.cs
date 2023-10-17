using AutoMapper;

namespace Acme.IdentityServer.WebApi.Assemblers.Implementors {
    public abstract class BaseModelAssembler {
        protected IMapper mapper {
            get {
                return MappingInitializer.Instance.Mapper;
            }
        }

        protected BaseModelAssembler() { }
    }
}
