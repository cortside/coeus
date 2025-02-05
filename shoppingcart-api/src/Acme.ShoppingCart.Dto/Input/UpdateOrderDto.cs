using System.Collections.Generic;

namespace Acme.ShoppingCart.Dto.Input {
    public class UpdateOrderDto {
        public AddressDto Address { get; set; }
        public List<UpdateOrderItemDto> Items { get; set; }
    }
}
