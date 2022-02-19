namespace Acme.ShoppingCart.Domain.Enumerations {
    public enum OrderStatus {
        Submitted,
        AwaitingValidation,
        StockConfirmed,
        Paid,
        Shipped,
        Cancelled
    }
}
