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

        public List<OrderItem> Items { get; private set; }

        private void Init() {
            OrderResourceId = Guid.NewGuid();
            Items = new List<OrderItem>();
        }

        public void AddItem(CatalogItem item, int quantity) {
            Items.Add(new OrderItem() { Sku = item.Sku, Quantity = quantity, UnitPrice = item.UnitPrice });
        }
    }
}
