﻿using Acme.ShoppingCart.CatalogApi.Models.Responses;
using Acme.ShoppingCart.WebApi.Models.Requests;
using Cortside.Common.Testing;

namespace Acme.ShoppingCart.TestUtilities {
    public class ModelBuilder {
        public static CatalogItem GetCatalogItem(Guid? itemId = null) {
            return new CatalogItem {
                ImageUrl = $"https://{RandomValues.CreateRandomString}.com/{RandomValues.CreateRandomNumberString}.jpg",
                ItemId = itemId ?? Guid.NewGuid(),
                Name = RandomValues.CreateRandomString(),
                Sku = RandomValues.CreateRandomString(),
                UnitPrice = RandomValues.Number(1, 9999)
            };
        }

        public static CreateOrderModel GetCreateOrderModel() {

            return new CreateOrderModel() {
                Customer = new UpdateOrderCustomerModel() {
                    FirstName = RandomValues.FirstName,
                    LastName = RandomValues.LastName,
                    Email = GetEmail()
                },
                Address = new WebApi.Models.AddressModel() {
                    Street = RandomValues.AddressLine1,
                    City = RandomValues.City,
                    State = RandomValues.State,
                    Country = "USA",
                    ZipCode = RandomValues.ZipCode
                },
                Items = [GetCreateOrderItemModel()]
            };
        }

        public static UpdateCustomerModel GetUpdateCustomerModel() {
            return new UpdateCustomerModel() {
                FirstName = RandomValues.FirstName,
                LastName = RandomValues.LastName,
                Email = GetEmail()
            };
        }

        public static CreateOrderItemModel GetCreateOrderItemModel() {
            return new CreateOrderItemModel() { Sku = RandomValues.CreateRandomString(), Quantity = RandomValues.Number(1, 50) };
        }

        public static string GetEmail() {
            var email = RandomValues.EmailAddress;
            return $"{email.Split('.')[0]}.com";
        }
    }
}
