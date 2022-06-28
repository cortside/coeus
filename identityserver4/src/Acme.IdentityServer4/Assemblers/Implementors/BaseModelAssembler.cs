using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Cortside.IdentityServer.WebApi.Assemblers.Implementors {
    public abstract class BaseModelAssembler {
        protected IMapper mapper {
            get {
                return MappingInitializer.Instance.Mapper;
            }
        }

        protected BaseModelAssembler() { }
    }
}
