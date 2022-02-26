#pragma warning disable CS1591

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.ShoppingCart.WebApi.Models.Requests {
    /// <summary>
    /// Represents a single loan
    /// </summary>
    public class CreateCustomerOrderModel {
        [Required]
        public AddressModel Address { get; set; }
        public List<CreateOrderItemModel> Items { get; set; }
    }
}
