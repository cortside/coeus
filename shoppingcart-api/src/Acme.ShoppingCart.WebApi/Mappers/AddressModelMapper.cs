using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.WebApi.Models;

namespace Acme.ShoppingCart.WebApi.Mappers {
    public class AddressModelMapper {
        public AddressModel Map(AddressDto dto) {
            if (dto == null) {
                return null;
            }

            return new AddressModel() {
                Street = dto.Street,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                ZipCode = dto.ZipCode
            };
        }
    }
}
