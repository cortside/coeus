using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acme.ShoppingCart.Domain.Entities {
    public class Address : AuditableEntity {
        protected Address() { }

        public Address(string street, string city, string state, string country, string zipcode) {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; private set; }

        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }

        public void Update(string street, string city, string state, string country, string zipcode) {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }
    }
}
