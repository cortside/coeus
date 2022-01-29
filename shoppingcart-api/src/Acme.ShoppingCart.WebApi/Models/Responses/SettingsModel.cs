using Cortside.Health.Models;

namespace Acme.ShoppingCart.WebApi.Models.Responses {
    /// <summary>
    /// Settings
    /// </summary>
    public class SettingsModel {
        /// <summary>
        /// Which service the settings came from
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Build
        /// </summary>
        public BuildModel Build { get; set; }

        /// <summary>
        /// Configuration options
        /// </summary>
        public ConfigurationModel Configuration { get; set; }
    }
}
