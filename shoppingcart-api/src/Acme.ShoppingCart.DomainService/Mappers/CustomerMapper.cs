using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.DomainService.Mappers {
    public class CustomerMapper {
        private readonly SubjectMapper subjectMapper;

        public CustomerMapper(SubjectMapper subjectMapper) {
            this.subjectMapper = subjectMapper;
        }

        public CustomerDto MapToDto(Customer entity) {
            if (entity == null) {
                return null;
            }

            return new CustomerDto {
                CustomerId = entity.CustomerId,
                CustomerResourceId = entity.CustomerResourceId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                CreatedSubject = subjectMapper.MapToDto(entity.CreatedSubject),
                LastModifiedSubject = subjectMapper.MapToDto(entity.LastModifiedSubject),
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
