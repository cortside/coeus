using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acme.ShoppingCart.Domain.Entities {
    [Table("Subject")]
    public class Subject {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid SubjectId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string GivenName { get; set; }

        [StringLength(100)]
        public string FamilyName { get; set; }

        [StringLength(100)]
        public string UserPrincipalName { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
