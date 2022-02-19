using System;

namespace Acme.ShoppingCart.WebApi.Models {
    public class AddressModel {
        public AddressModel() { }

        public String Street { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Country { get; set; }
        public String ZipCode { get; set; }
    }
}
