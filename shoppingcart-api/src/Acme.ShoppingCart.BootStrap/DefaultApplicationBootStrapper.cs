using System.Collections.Generic;
using Acme.ShoppingCart.BootStrap.Installer;
using Cortside.Common.BootStrap;

namespace Acme.ShoppingCart.BootStrap {
    public class DefaultApplicationBootStrapper : BootStrapper {
        public DefaultApplicationBootStrapper() {
            installers = new List<IInstaller> {
                new ExampleHostedServiceInstaller(),
                new RepositoryInstaller(),
                new DomainServiceInstaller(),
                new DistributedLockInstaller(),
                new RestApiClientInstaller(),
                new FacadeInstaller()
            };
        }
    }
}
