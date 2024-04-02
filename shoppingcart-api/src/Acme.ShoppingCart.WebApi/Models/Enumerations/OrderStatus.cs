namespace Acme.ShoppingCart.WebApi.Models.Enumerations {
    /// <summary>
    /// Status/states that an order may be in
    /// </summary>
    public enum OrderStatus {
        /// <summary>
        /// The order has been created without any another moving action
        /// </summary>
        Created,
        /// <summary>
        /// The order has been paid for and can no longer be changed
        /// </summary>
        Paid,
        /// <summary>
        /// The order has been shipped and can no longer be cancelled
        /// </summary>
        Shipped,
        /// <summary>
        /// The order has been cancelled
        /// </summary>
        Cancelled
    }
}
