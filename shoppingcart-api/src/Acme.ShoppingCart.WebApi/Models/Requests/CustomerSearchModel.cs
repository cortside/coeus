using System;
using Cortside.AspNetCore.Common.Models;

namespace Acme.ShoppingCart.WebApi.Models.Requests {
    /// <summary>
    /// Request for customer search options
    /// </summary>
    public class CustomerSearchModel : SearchModel {
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
    }
}
