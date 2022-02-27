using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.DomainService.Mappers {
    public class AddressMapper {
        private readonly SubjectMapper subjectMapper;

        public AddressMapper(SubjectMapper subjectMapper) {
            this.subjectMapper = subjectMapper;
        }

        public AddressDto MapToDto(Address entity) {
            if (entity == null) {
                return null;
            }

            return new AddressDto() {
                Street = entity.Street,
                City = entity.City,
                State = entity.State,
                Country = entity.Country,
                ZipCode = entity.ZipCode
            };
        }

        //public void UpdateCustomerEntity(Customer entity, CustomerDto dto) {
        //    entity.FirstName = dto.FirstName;
        //    entity.LastName = dto.LastName;
        //    entity.Age = dto.Age;
        //    this.addressServiceMapper.UpdateAddress(entity.Address, dto.Address);
        //}
    }
}
