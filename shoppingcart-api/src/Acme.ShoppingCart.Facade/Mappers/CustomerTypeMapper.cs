using Acme.ShoppingCart.Domain.Entities;
using Acme.ShoppingCart.Dto;

namespace Acme.ShoppingCart.Facade.Mappers {
    public class CustomerTypeMapper {
        public CustomerTypeDto MapToDto(CustomerType entity) {
            if (entity == null) {
                return null;
            }

            return new CustomerTypeDto {
                CustomerTypeId = entity.CustomerTypeId,
                Name = entity.Name,
                Description = entity.Description,
                TaxExempt = entity.TaxExempt
            };
        }
    }
}
