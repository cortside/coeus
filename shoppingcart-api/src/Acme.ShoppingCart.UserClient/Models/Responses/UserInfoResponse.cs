using System;

namespace Acme.ShoppingCart.UserClient.Models.Responses {
    public class UserInfoResponse {
        /// <summary>
        /// User Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Current status of user. Typically ACTIVE.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// First name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// User's email address.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Communication method.
        /// </summary>
        public string CommunicationMethod { get; set; }

        /// <summary>
        /// Date user was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
