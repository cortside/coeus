using System;

namespace Acme.ShoppingCart.WebApi.Models.Responses {
    public class OrderItemModel : AuditableEntityModel {
        public int OrderItemId { get; set; }
        public Guid ItemId { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
