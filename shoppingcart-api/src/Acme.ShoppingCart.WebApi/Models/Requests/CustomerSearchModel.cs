using System;

namespace Acme.ShoppingCart.WebApi.Models.Requests {
    public class CustomerSearchModel {
        public Guid? CustomerResourceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 30;
        public string Sort { get; set; } = string.Empty;
    }
}
