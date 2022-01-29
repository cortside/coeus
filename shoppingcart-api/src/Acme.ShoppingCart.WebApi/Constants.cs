#pragma warning disable CS1591

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
                public const string CreateWidget = "CreateWidget";
                public const string UpdateWidget = "UpdateWidget";
                public const string GetWidget = "GetWidget";
                public const string GetWidgets = "GetWidgets";
            }
        }
    }
}
