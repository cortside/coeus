using System.Collections.Generic;
using Cortside.Common.BootStrap;

namespace Acme.IdentityServer.BootStrap {
    public class DefaultApplicationBootStrapper : BootStrapper {
        public DefaultApplicationBootStrapper() {
            installers = new List<IInstaller>();
        }
    }
}
