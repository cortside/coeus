using System;

namespace Acme.ShoppingCart.Dto.Input {
    public class UpdateCustomerDto {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateOnly BirthDate { get; set; }
    }
}
