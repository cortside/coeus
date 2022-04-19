using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acme.ShoppingCart.Domain.Enumerations;
using Acme.ShoppingCart.UserClient.Models.Responses;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.Common.Validation;

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
            items = new List<OrderItem>();
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

        // expose items as a read only collection so that the collection cannot be manipulated without going through order
        private readonly List<OrderItem> items = new List<OrderItem>();
        public virtual IReadOnlyList<OrderItem> Items => items;

        public void AddItem(CatalogItem item, int quantity) {
            Guard.Against(() => Status == OrderStatus.Cancelled || Status == OrderStatus.Shipped, () => throw new InvalidOperationException($"Update not allowed when Status is {Status}"));

            var orderItem = items.Find(x => x.ItemId == item.ItemId);
            if (orderItem != null) {
                orderItem.AddQuantity(quantity);
            } else {
                items.Add(new OrderItem(item, quantity));
            }
        }

        public void UpdateAddress(string street, string city, string state, string country, string zipCode) {
            Guard.Against(() => Status == OrderStatus.Cancelled || Status == OrderStatus.Shipped, () => throw new InvalidOperationException($"Update not allowed when Status is {Status}"));
            Address.Update(street, city, state, country, zipCode);
        }
    }
}
