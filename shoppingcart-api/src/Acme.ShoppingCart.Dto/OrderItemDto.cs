namespace Acme.ShoppingCart.Dto {
    public class OrderItemDto {
        public int OrderItemId { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
