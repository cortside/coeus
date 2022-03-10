using System.Collections.Generic;
using Acme.ShoppingCart.BootStrap.Installer;
using Cortside.Common.BootStrap;

namespace Acme.ShoppingCart.BootStrap {
    public class DefaultApplicationBootStrapper : BootStrapper {
        public DefaultApplicationBootStrapper() {
            installers = new List<IInstaller> {
                new DomainEventInstaller(),
                new ExampleHostedServiceInstaller(),
                new HealthInstaller(),
                new DbContextInstaller(),
                new DomainInstaller(),
                new MiniProfilerInstaller(),
                new DistributedLockInstaller(),
                new EncryptionInstaller(),
                new CatalogClientInstaller(),
                new FacadeInstaller()
            };
        }
    }
}
