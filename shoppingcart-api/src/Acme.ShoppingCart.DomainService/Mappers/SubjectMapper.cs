using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.DomainService.Mappers {
    public class SubjectMapper {
        public SubjectDto MapToDto(Subject entity) {
            if (entity == null) {
                return null;
            }

            return new SubjectDto() {
                SubjectId = entity.SubjectId,
                GivenName = entity.GivenName,
                FamilyName = entity.FamilyName,
                Name = entity.Name,
                UserPrincipalName = entity.UserPrincipalName
            };
        }

        //public void UpdateCustomerEntity(Customer entity, CustomerDto dto) {
        //    //entity.FirstName = dto.FirstName;
        //    //entity.LastName = dto.LastName;
        //    //entity.Age = dto.Age;
        //    //this.addressServiceMapper.UpdateAddress(entity.Address, dto.Address);
        //}
    }
}
