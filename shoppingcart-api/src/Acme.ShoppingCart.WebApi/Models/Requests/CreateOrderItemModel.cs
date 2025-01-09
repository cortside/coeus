using System.ComponentModel.DataAnnotations;

namespace Acme.ShoppingCart.WebApi.Models.Requests {
    /// <summary>
    /// Request to add an item to an order
    /// </summary>
    public class CreateOrderItemModel {
        /// <summary>
        /// Gets or sets the sku.
        /// </summary>
        /// <value>
        /// The sku.
        /// </value>
        [Required]
        public string Sku { get; set; }
        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        [Required]
        public int? Quantity { get; set; }
    }
}
