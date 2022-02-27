using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acme.ShoppingCart.UserClient.Models.Responses;

namespace Acme.ShoppingCart.Domain.Entities {
    [Table("OrderItem")]
    public class OrderItem : AuditableEntity {
        protected OrderItem() {
            // Required by EF as it doesn't know about CatalogItem
        }

        public OrderItem(CatalogItem item, int quantity) {
            ItemId = item.ItemId;
            Sku = item.Sku;
            Quantity = quantity;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; private set; }
        public Guid ItemId { get; private set; }
        [StringLength(10)]
        public string Sku { get; private set; }
        public int Quantity { get; private set; }
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; private set; }
    }
}
