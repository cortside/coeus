using System.Collections.Generic;

namespace Acme.ShoppingCart.Dto.Input {
    public class CreateOrderDto {
        public CreateOrderCustomerDto Customer { get; set; }
        public AddressDto Address { get; set; }
        public List<UpdateOrderItemDto> Items { get; set; }
    }
}
