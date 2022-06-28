using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cortside.IdentityServer.WebApi.Data {
    public abstract class AuditableEntity {
        public Guid CreateUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid LastModifiedUserId { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
