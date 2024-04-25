using System;
using System.Collections.Generic;
using Acme.ShoppingCart.Enumerations;
using Cortside.AspNetCore.Common.Models;

namespace Acme.ShoppingCart.WebApi.Models.Responses {
    /// <summary>
    /// Represents a single order
    /// </summary>
    public class OrderModel : AuditableEntityModel {
        /// <summary>
        /// Gets or sets the order resource identifier.
        /// </summary>
        /// <value>
        /// The order resource identifier.
        /// </value>
        public Guid OrderResourceId { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public OrderStatus Status { get; set; }
        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        /// <value>
        /// The customer.
        /// </value>
        public CustomerModel Customer { get; set; }
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public AddressModel Address { get; set; }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<OrderItemModel> Items { get; set; }
    }
}
