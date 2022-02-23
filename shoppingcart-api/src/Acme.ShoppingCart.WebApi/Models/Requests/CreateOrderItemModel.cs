namespace Acme.ShoppingCart.WebApi.Models.Requests {
    public class CreateOrderItemModel {
        public string Sku { get; set; }
        public int Quantity { get; set; }
    }
}
