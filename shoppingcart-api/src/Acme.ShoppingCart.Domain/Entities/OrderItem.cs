using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acme.ShoppingCart.CatalogApi.Models.Responses;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.Common.Validation;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.Domain.Entities {
    [Table("OrderItem")]
    [Comment("Items that belong to an Order")]
    public class OrderItem : AuditableEntity {
        protected OrderItem() {
            // Required by EF as it doesn't know about CatalogItem
        }

        public OrderItem(CatalogItem item, int quantity) {
            Guard.From.Null(item, nameof(item));
            Guard.Against(() => quantity < 0, () => throw new ArgumentException($"Quantity of {quantity} is invalid"));

            ItemId = item.ItemId;
            Sku = item.Sku;
            Quantity = quantity;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Comment("Primary Key")]
        public int OrderItemId { get; private set; }

        /// <summary>
        /// FK to Order that the OrderItem belongs to
        /// </summary>
        /// <remarks>OrderId added explicitly here so that it does not become nullable when inferred by relationships</remarks>
        [Comment("FK to Order that the OrderItem belongs to")]
        [ForeignKey(nameof(OrderId))]
        public int OrderId { get; private set; }

        [Comment("FK to Item in Catalog service")]
        public Guid ItemId { get; private set; }

        [Required]
        [StringLength(10)]
        [Comment("Item Sku")]
        public string Sku { get; private set; }

        [Comment("Quantity of Sku")]
        public int Quantity { get; private set; }

        [Column(TypeName = "money")]
        [Comment("Per quantity price")]
        public decimal UnitPrice { get; private set; }

        internal void AddQuantity(int quantity) {
            Guard.Against(() => Quantity < 0, () => throw new ArgumentException($"Item quantity of {Quantity} is invalid"));
            Guard.Against(() => quantity < 0, () => throw new ArgumentException($"Quantity of {quantity} is invalid"));

            Quantity += quantity;
        }

        internal void UpdateQuantity(int quantity) {
            Guard.Against(() => quantity < 0, () => throw new ArgumentException($"Quantity of {quantity} is invalid"));

            Quantity = quantity;
        }
    }
}
