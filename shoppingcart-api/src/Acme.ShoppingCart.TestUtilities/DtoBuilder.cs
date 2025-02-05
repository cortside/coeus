using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.Dto.Input;
using Cortside.Common.Testing;

namespace Acme.ShoppingCart.TestUtilities {
    public static class DtoBuilder {
        public static UpdateCustomerDto GetUpdateCustomerDto() {
            return new UpdateCustomerDto() {
                FirstName = RandomValues.FirstName,
                LastName = RandomValues.LastName,
                Email = ModelBuilder.GetEmail()
            };
        }

        public static CreateOrderDto GetCreateOrderDto() {
            return new CreateOrderDto() {
                Address = new AddressDto() {
                    Street = RandomValues.AddressLine1,
                    City = RandomValues.City,
                    State = RandomValues.State,
                    ZipCode = RandomValues.ZipCode,
                    Country = "USA"
                },
                Items = []
            };
        }
    }
}
