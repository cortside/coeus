#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using Acme.ShoppingCart.WebApi.Models.Enumerations;

namespace Acme.ShoppingCart.WebApi.Models.Responses {
    /// <summary>
    /// Represents a single loan
    /// </summary>
    public class OrderModel : AuditableEntityModel {
        public Guid OrderResourceId { get; set; }
        public OrderStatus Status { get; set; }
        public CustomerModel Customer { get; set; }
        public AddressModel Address { get; set; }
        public List<OrderItemModel> Items { get; set; }
    }
}
