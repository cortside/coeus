#pragma warning disable CS1591

using System.ComponentModel.DataAnnotations;

namespace Acme.ShoppingCart.WebApi.Models.Requests {
    /// <summary>
    /// Represents a single loan
    /// </summary>
    public class CustomerRequest {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
