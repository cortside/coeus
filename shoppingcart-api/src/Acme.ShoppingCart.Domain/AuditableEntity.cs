using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acme.ShoppingCart.Domain {
    /// <summary>
    /// Auditable entity base class
    /// </summary>
    public abstract class AuditableEntity {
        public DateTime CreatedDate { get; set; }

        [ForeignKey("CreateSubjectId")]
        [Required]
        public virtual Subject CreatedSubject { get; set; }

        public DateTime LastModifiedDate { get; set; }

        [ForeignKey("LastModifiedSubjectId")]
        [Required]
        public virtual Subject LastModifiedSubject { get; set; }
    }
}
