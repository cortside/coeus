using Acme.ShoppingCart.Dto;
using Acme.ShoppingCart.WebApi.Models.Responses;

namespace Acme.ShoppingCart.WebApi.Mappers {
    public class OrderModelMapper {
        private readonly CustomerModelMapper customerMapper;
        private readonly AddressModelMapper addressMapper;
        private readonly SubjectModelMapper subjectMapper;

        public OrderModelMapper(CustomerModelMapper customerMapper, AddressModelMapper addressMapper, SubjectModelMapper subjectMapper) {
            this.customerMapper = customerMapper;
            this.addressMapper = addressMapper;
            this.subjectMapper = subjectMapper;
        }

        public OrderModel Map(OrderDto dto) {
            if (dto == null) {
                return null;
            }

            var model = new OrderModel() {
                OrderResourceId = dto.OrderResourceId,
                Address = addressMapper.Map(dto.Address),
                Items = dto.Items.ConvertAll(x => Map(x)),
                Customer = customerMapper.Map(dto.Customer),
                CreatedDate = dto.CreatedDate,
                LastModifiedDate = dto.LastModifiedDate,
                CreatedSubject = subjectMapper.Map(dto.CreatedSubject),
                LastModifiedSubject = subjectMapper.Map(dto.LastModifiedSubject)
            };

            return model;
        }

        public OrderItemModel Map(OrderItemDto dto) {
            if (dto == null) {
                return null;
            }

            return new OrderItemModel {
                OrderItemId = dto.OrderItemId,
                ItemId = dto.ItemId,
                Sku = dto.Sku,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };
        }
    }
}
