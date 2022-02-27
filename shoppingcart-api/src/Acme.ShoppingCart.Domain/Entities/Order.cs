using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acme.ShoppingCart.Domain.Enumerations;
using Acme.ShoppingCart.UserClient.Models.Responses;

namespace Acme.ShoppingCart.Domain.Entities {
    [Table("Order")]
    public class Order : AuditableEntity {
        protected Order() {
            // Required by EF as it doesn't know about Customer
        }

        public Order(Customer customer, string street, string city, string state, string country, string zipCode) {
            OrderResourceId = Guid.NewGuid();
            Customer = customer;
            Address = new Address(street, city, state, country, zipCode);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; private set; }

        public Guid OrderResourceId { get; private set; }

        [Column(TypeName = "nvarchar(20)")]
        public OrderStatus Status { get; private set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; private set; }

        [ForeignKey("AddressId")]
        public Address Address { get; private set; }

        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();

        public void AddItem(CatalogItem item, int quantity) {
            Items.Add(new OrderItem(item, quantity));
        }
    }
}
