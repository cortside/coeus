using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acme.ShoppingCart.Domain.Enumerations;

namespace Acme.ShoppingCart.Domain.Entities {
    [Table("Order")]
    public class Order : AuditableEntity {
        public Order() {
            OrderResourceId = Guid.NewGuid();
            Items = new List<OrderItem>();
        }

        public Order(Customer customer, string street, string city, string state, string country, string zipCode) : base() {
            Customer = customer;
            Address = new Address(street, city, state, country, zipCode);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        public Guid OrderResourceId { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public OrderStatus Status { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        [ForeignKey("AddressId")]
        public Address Address { get; set; }

        public List<OrderItem> Items { get; set; }

        public void AddItem(string sku, int quantity) {
            Items.Add(new OrderItem() { Sku = sku, Quantity = quantity });
        }
    }
}
