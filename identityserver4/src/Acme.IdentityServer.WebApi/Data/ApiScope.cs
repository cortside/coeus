using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acme.IdentityServer.WebApi.Data {
    public class ApiScope {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ApiResourceId { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool Emphasize { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public string ShowInDiscoveryDocument { get; set; }
        public bool Enabled { get; set; }
        public int? ApiScopeId { get; set; }

    }
}
