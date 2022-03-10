using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.Facade.Mappers {
    public class OrderMapper {
        private readonly CustomerMapper customerMapper;
        private readonly AddressMapper addressMapper;
        private readonly SubjectMapper subjectMapper;

        public OrderMapper(CustomerMapper customerMapper, AddressMapper addressMapper, SubjectMapper subjectMapper) {
            this.customerMapper = customerMapper;
            this.addressMapper = addressMapper;
            this.subjectMapper = subjectMapper;
        }

        public OrderDto? MapToDto(Order entity) {
            if (entity == null) {
                return null;
            }

            var dto = new OrderDto() {
                OrderId = entity.OrderId,
                OrderResourceId = entity.OrderResourceId,
                Address = addressMapper.MapToDto(entity.Address),
                Items = entity.Items.ConvertAll(x => MapToDto(x)),
                Customer = customerMapper.MapToDto(entity.Customer),
                CreatedDate = entity.CreatedDate,
                LastModifiedDate = entity.LastModifiedDate,
                CreatedSubject = subjectMapper.MapToDto(entity.CreatedSubject),
                LastModifiedSubject = subjectMapper.MapToDto(entity.LastModifiedSubject)
            };

            return dto;
        }

        public OrderItemDto? MapToDto(OrderItem entity) {
            if (entity == null) {
                return null;
            }

            return new OrderItemDto {
                OrderItemId = entity.OrderItemId,
                Sku = entity.Sku,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                CreatedDate = entity.CreatedDate,
                CreatedSubject = subjectMapper.MapToDto(entity.CreatedSubject),
                LastModifiedDate = entity.LastModifiedDate,
                LastModifiedSubject = subjectMapper.MapToDto(entity.LastModifiedSubject)
            };
        }
    }
}
