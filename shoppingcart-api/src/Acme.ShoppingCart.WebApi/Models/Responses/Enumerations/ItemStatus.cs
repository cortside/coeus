namespace Acme.ShoppingCart.WebApi.Models.Responses.Enumerations {
    /// <summary>
    /// Represents the status of an item in the shopping cart.
    /// </summary>
    public enum ItemStatus {
        /// <summary>
        /// The item is active and available for purchase.
        /// </summary>
        Active,

        /// <summary>
        /// The item is unavailable for purchase.
        /// </summary>
        Unavailable
    }
}
