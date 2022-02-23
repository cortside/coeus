using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acme.ShoppingCart.Domain.Enumerations;
using Acme.ShoppingCart.UserClient.Models.Responses;

namespace Acme.ShoppingCart.Domain.Entities {
    [Table("Order")]
    public class Order : AuditableEntity {
        public Order() {
            Init();
        }

        public Order(Customer customer, string street, string city, string state, string country, string zipCode) {
            Init();
            Customer = customer;
            Address = new Address(street, city, state, country, zipCode);
        }

        private void Init() {
            OrderResourceId = Guid.NewGuid();
            Items = new List<OrderItem>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        public Guid OrderResourceId { get; set; }

        [StringLength(10)]
        public OrderStatus Status { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        [ForeignKey("AddressId")]
        public Address Address { get; set; }

        public List<OrderItem> Items { get; set; }

        public void AddItem(CatalogItem item, int quantity) {
            Items.Add(new OrderItem() { Sku = item.Sku, Quantity = quantity, UnitPrice = item.UnitPrice });
        }
    }
}
