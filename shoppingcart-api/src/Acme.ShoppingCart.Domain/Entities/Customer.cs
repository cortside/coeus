using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cortside.AspNetCore.Auditable.Entities;

namespace Acme.ShoppingCart.Domain.Entities {
    [Table("Customer")]
    public class Customer : AuditableEntity {
        public Customer(string firstName, string lastName, string email) {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            CustomerResourceId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; private set; }

        public Guid CustomerResourceId { get; private set; }

        [StringLength(50)]
        public string FirstName { get; private set; }
        [StringLength(50)]
        public string LastName { get; private set; }
        [StringLength(250)]
        public string Email { get; private set; }

        public void Update(string firstName, string lastName, string email) {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}
