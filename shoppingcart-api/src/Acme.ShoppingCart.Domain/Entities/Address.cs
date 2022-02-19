using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acme.ShoppingCart.Domain.Entities {
    public class Address : AuditableEntity {  //} : ValueObject {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }

        public Address() { }

        public Address(string street, string city, string state, string country, string zipcode) {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }

        //protected override IEnumerable<object> GetEqualityComponents() {
        //    // Using a yield return statement to return each element one at a time
        //    yield return Street;
        //    yield return City;
        //    yield return State;
        //    yield return Country;
        //    yield return ZipCode;
        //}
    }
}
