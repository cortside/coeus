using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acme.ShoppingCart.Domain.Entities {
    [Table("Customer")]
    public class Customer : AuditableEntity {
        public Customer() {
            CustomerResourceId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        public Guid CustomerResourceId { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(250)]
        public string Email { get; set; }
    }
}
