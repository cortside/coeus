using System.Collections.Generic;
using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.DomainService.Mappers {
    public class OrderMapper {
        private readonly CustomerMapper customerMapper;
        private readonly AddressMapper addressMapper;
        private readonly SubjectMapper subjectMapper;

        public OrderMapper(CustomerMapper customerMapper, AddressMapper addressMapper, SubjectMapper subjectMapper) {
            this.customerMapper = customerMapper;
            this.addressMapper = addressMapper;
            this.subjectMapper = subjectMapper;
        }

        public OrderDto MapToDto(Order entity) {
            if (entity == null) {
                return null;
            }

            var dto = new OrderDto() {
                OrderId = entity.OrderId,
                OrderResourceId = entity.OrderResourceId,
                Address = addressMapper.MapToDto(entity.Address),
                Items = new List<OrderItemDto>(),
                Customer = customerMapper.MapToDto(entity.Customer),
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                CreatedSubject = subjectMapper.MapToDto(entity.CreatedSubject),
                LastModifiedSubject = subjectMapper.MapToDto(entity.LastModifiedSubject)
            };

            return dto;
        }

        public void UpdateCustomer(Order entity, OrderDto dto) {
            //entity.FirstName = dto.FirstName;
            //entity.LastName = dto.LastName;
            //entity.Age = dto.Age;
            //this.addressServiceMapper.UpdateAddress(entity.Address, dto.Address);
        }
    }
}
