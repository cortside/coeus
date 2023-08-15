using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cortside.AspNetCore.Auditable.Entities;

namespace Acme.ShoppingCart.Domain.Entities {
    public class CustomerType : AuditableEntity {
        protected CustomerType() { }

        public CustomerType(string name, string description, bool taxExempt) {
            Name = name;
            Description = description;
            TaxExempt = taxExempt;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerTypeId { get; private set; }

        [StringLength(50)]
        public string Name { get; private set; }
        [StringLength(100)]
        public string Description { get; private set; }
        public bool TaxExempt { get; private set; }
    }
}
