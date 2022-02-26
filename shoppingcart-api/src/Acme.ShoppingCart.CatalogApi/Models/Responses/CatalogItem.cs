using System;

namespace Acme.ShoppingCart.UserClient.Models.Responses {
    public class CatalogItem {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
