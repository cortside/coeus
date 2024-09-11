#pragma warning disable CS1591 // Missing XML comments

using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.WebApi.Models.Requests;
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

        public CustomerSearchDto MapToDto(CustomerSearchModel model) {
            if (model == null) {
                return null;
            }

            return new CustomerSearchDto() {
                CustomerResourceId = model.CustomerResourceId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
                Sort = model.Sort
            };
        }

        public UpdateCustomerDto MapToDto(UpdateCustomerModel model) {
            if (model == null) {
                return null;
            }

            return new UpdateCustomerDto() {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email
            };
        }
    }
}
