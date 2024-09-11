using System;
using Cortside.AspNetCore.Common.Models;

namespace Acme.ShoppingCart.WebApi.Models.Requests {
    /// <summary>
    /// Order search
    /// </summary>
    public class OrderSearchModel : SearchModel {
        /// <summary>
        /// Customer's identifier
        /// </summary>
        public Guid? CustomerResourceId { get; set; }

        /// <summary>
        /// Customer first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Customer last name
        /// </summary>
        public string LastName { get; set; }
    }
}
