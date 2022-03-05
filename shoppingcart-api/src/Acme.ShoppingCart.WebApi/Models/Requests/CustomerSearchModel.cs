using System;

namespace Acme.ShoppingCart.WebApi.Models.Requests {
    /// <summary>
    /// Request for customer search options
    /// </summary>
    public class CustomerSearchModel {
        /// <summary>
        /// Gets or sets the customer resource identifier.
        /// </summary>
        /// <value>
        /// The customer resource identifier.
        /// </value>
        public Guid? CustomerResourceId { get; set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNumber { get; set; } = 1;
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; } = 30;
        /// <summary>
        /// Gets or sets the sort.
        /// </summary>
        /// <value>
        /// The sort.
        /// </value>
        public string Sort { get; set; } = string.Empty;
    }
}
