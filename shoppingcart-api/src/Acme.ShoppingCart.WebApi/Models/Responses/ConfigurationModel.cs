namespace Acme.ShoppingCart.WebApi.Models.Responses {
    /// <summary>
    /// Configuration
    /// </summary>
    public class ConfigurationModel {
        /// <summary>
        /// Service bus configuration
        /// </summary>
        public ServicebusModel ServiceBus { get; set; }
        /// <summary>
        /// Identity Server configuration
        /// </summary>
        public IdentityServerModel IdentityServer { get; set; }
        /// <summary>
        /// Policy Server configuration
        /// </summary>
        public PolicyServerModel PolicyServer { get; set; }
    }
}
