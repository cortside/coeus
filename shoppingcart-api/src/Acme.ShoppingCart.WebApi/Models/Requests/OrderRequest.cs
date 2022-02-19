#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.ShoppingCart.WebApi.Models.Requests {
    /// <summary>
    /// Represents a single loan
    /// </summary>
    public class OrderRequest {
        [Required]
        public Guid CustomerResourceId { get; set; }
        [Required]
        public AddressModel Address { get; set; }
        public List<OrderItemModel> Items { get; set; }
    }
}
