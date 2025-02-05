namespace Acme.ShoppingCart.WebApi.Models.Enumerations {
    /// <summary>
    /// Order status
    /// </summary>
    public enum OrderStatus {
        /// <summary>
        /// Created
        /// </summary>
        Created,
        /// <summary>
        /// Paid
        /// </summary>
        Paid,
        /// <summary>
        /// Shipped
        /// </summary>
        Shipped,
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled
    }
}
