using System;
using Cortside.AspNetCore.Common.Dtos;

namespace Acme.ShoppingCart.Dto.Output {
    public class OrderItemDto : AuditableEntityDto {
        public int OrderItemId { get; set; }
        public Guid ItemId { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
