#pragma warning disable CA1724 // Unused type parameters should be removed
#pragma warning disable CA1034 // Do not nest public type
#pragma warning disable CS1591 // Missing XML comments

namespace Acme.ShoppingCart.WebApi {
    /// <summary>
    /// Constanst for webapi
    /// </summary>
    public static class Constants {
        /// <summary>
        /// Authorization constants
        /// </summary>
        public static class Authorization {
            /// <summary>
            /// Permission constants
            /// </summary>
            public static class Permissions {
                public const string CreateCustomer = "CreateCustomer";
                public const string UpdateCustomer = "UpdateCustomer";
                public const string GetCustomer = "GetCustomer";
                public const string GetCustomers = "GetCustomers";
                public const string PublishCustomer = "PublishCustomer";
                public const string GetOrder = "GetOrder";
                public const string GetOrders = "GetOrders";
                public const string CreateOrder = "CreateOrder";
                public const string UpdateOrder = "UpdateOrder";
                public const string PublishOrder = "PublishOrder";
            }
        }
    }
}
