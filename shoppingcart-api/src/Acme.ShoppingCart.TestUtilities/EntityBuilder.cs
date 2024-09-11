using Acme.ShoppingCart.CatalogApi.Models.Responses;
using Acme.ShoppingCart.Domain.Entities;
using Cortside.Common.Testing;

namespace Acme.ShoppingCart.TestUtilities {
    public class EntityBuilder {
        public static Customer GetCustomerEntity() {
            return new Customer(RandomValues.FirstName, RandomValues.LastName, ModelBuilder.GetEmail());
        }

        public static Address GetAddressEntity() {
            return new Address(RandomValues.AddressLine1, RandomValues.City, RandomValues.State, "U.S.A.", RandomValues.ZipCode);
        }

        public static Order GetOrderEntity(Customer customer = null) {
            return new Order(customer ?? GetCustomerEntity(), RandomValues.AddressLine1, RandomValues.City, RandomValues.State, "U.S.A.", RandomValues.ZipCode);
        }

        public static OrderItem GetOrderItemEntity(CatalogItem catalogItem = null) {
            return new OrderItem(catalogItem ?? ModelBuilder.GetCatalogItem(), RandomValues.Number(1, 50));
        }
    }
}
