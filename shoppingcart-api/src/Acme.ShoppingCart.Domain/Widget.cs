using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acme.ShoppingCart.Domain {
    [Table("Widget")]
    public class Widget : AuditableEntity {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WidgetId { get; set; }

        [StringLength(100)]
        public string Text { get; set; }

        [Required]
        public int Width { get; set; }
        [Required]
        public int Height { get; set; }
    }
}
