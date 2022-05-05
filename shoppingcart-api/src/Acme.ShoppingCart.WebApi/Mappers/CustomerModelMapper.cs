using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.WebApi.Models.Responses;

namespace Acme.ShoppingCart.WebApi.Mappers {
    public class CustomerModelMapper {
        private readonly SubjectModelMapper subjectModelMapper;

        public CustomerModelMapper(SubjectModelMapper subjectModelMapper) {
            this.subjectModelMapper = subjectModelMapper;
        }

        public CustomerModel Map(CustomerDto dto) {
            if (dto == null) {
                return null;
            }

            return new CustomerModel {
                CustomerResourceId = dto.CustomerResourceId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                CreatedDate = dto.CreatedDate,
                CreatedSubject = subjectModelMapper.Map(dto.CreatedSubject),
                LastModifiedDate = dto.LastModifiedDate,
                LastModifiedSubject = subjectModelMapper.Map(dto.LastModifiedSubject)
            };
        }
    }
}
