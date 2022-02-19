using System;

namespace Acme.ShoppingCart.UserClient.Models.Responses {
    public class CatalogItemResponse {
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string CommunicationMethod { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
