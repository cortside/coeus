using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Acme.ShoppingCart.CatalogApi.Models.Responses;
using Acme.ShoppingCart.Domain.Enumerations;
using Acme.ShoppingCart.Exceptions;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.Common.Validation;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.Domain.Entities {
    [Index(nameof(OrderResourceId), IsUnique = true)]
    [Table("Order")]
    [Comment("Orders")]
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
        [Comment("Primary Key")]
        public int OrderId { get; private set; }

        [Comment("Public unique identifier")]
        public Guid OrderResourceId { get; private set; }

        [Column(TypeName = "nvarchar(20)")]
        [Comment("Order status (created, paid, shipped, cancelled)")]
        public OrderStatus Status { get; private set; }

        [ForeignKey("CustomerId")]
        [Comment("FK to Customer")]
        public Customer Customer { get; private set; }

        [Comment("FK to Address")]
        [ForeignKey("AddressId")]
        public Address Address { get; private set; }

        [Comment("Date customer was last notified for order")]
        public DateTime? LastNotified { get; private set; }

        // expose items as a read only collection so that the collection cannot be manipulated without going through order
        private readonly List<OrderItem> items = new List<OrderItem>();
        public virtual IReadOnlyList<OrderItem> Items => items;

        private void AssertOpenOrder() {
            Guard.Against(() => Status == OrderStatus.Cancelled || Status == OrderStatus.Shipped, () => throw new InvalidOrderStateChangeMessage($"Update not allowed when Status is {Status}"));
        }

        public void UpdateAddress(string street, string city, string state, string country, string zipCode) {
            AssertOpenOrder();

            Address.Update(street, city, state, country, zipCode);
        }

        public void AddItem(CatalogItem item, int quantity) {
            AssertOpenOrder();

            var orderItem = items.Find(x => x.ItemId == item.ItemId);
            if (orderItem != null) {
                orderItem.AddQuantity(quantity);
            } else {
                items.Add(new OrderItem(item, quantity));
            }
        }

        public void RemoveItem(OrderItem item) {
            AssertOpenOrder();
            Guard.Against(() => item == null || !items.Contains(item), () => throw new InvalidItemMessage("Item to remove must not be null and must be part of order"));

            items.Remove(item);
        }

        public void RemoveItems(List<OrderItem> itemsToRemove) {
            AssertOpenOrder();
            Guard.Against(() => itemsToRemove == null || itemsToRemove.Count == 0, () => throw new InvalidItemMessage("Items to remove must not be null and have items"));
            foreach (var item in itemsToRemove) {
                Guard.Against(() => item == null || !items.Contains(item), () => throw new InvalidItemMessage("Item to remove must not be null and must be part of order"));
            }

            foreach (var item in itemsToRemove) {
                items.Remove(item);
            }
        }

        public void UpdateItem(OrderItem item, int quantity) {
            AssertOpenOrder();
            Guard.Against(() => item == null || !items.Contains(item), () => throw new InvalidItemMessage("Item to remove must not be null and must be part of order"));

            item.UpdateQuantity(quantity);
        }

        public void SendNotification() {
            LastNotified = DateTime.UtcNow;
        }
    }
}
