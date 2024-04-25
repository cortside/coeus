using System;

namespace Acme.ShoppingCart.Dto {
    public class OrderSearchDto : SearchDto {
        public Guid? CustomerResourceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
