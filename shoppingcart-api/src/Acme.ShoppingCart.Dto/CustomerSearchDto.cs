using System;
using Cortside.AspNetCore.Common.Dtos;

namespace Acme.ShoppingCart.Dto {
    public class CustomerSearchDto : SearchDto {
        public Guid? CustomerResourceId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
