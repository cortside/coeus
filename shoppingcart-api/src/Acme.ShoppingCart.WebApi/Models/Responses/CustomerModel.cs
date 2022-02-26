#pragma warning disable CS1591

using System;

namespace Acme.ShoppingCart.WebApi.Models.Responses {
    /// <summary>
    /// Represents a single customer
    /// </summary>
    public class CustomerModel : AuditableEntityModel {
        public Guid CustomerResourceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
