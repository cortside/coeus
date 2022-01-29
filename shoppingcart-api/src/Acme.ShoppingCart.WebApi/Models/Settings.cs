using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Acme.ShoppingCart.WebApi.Models {
    /// <summary>
    /// Build information
    /// </summary>
    public class Build {
        /// <summary>
        /// Build version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Build tag
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Build date
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Settings response
    /// </summary>
    public class Settings {
        /// <summary>
        /// Deployment environment for service
        /// </summary>
        public string Deployment { get; set; }

        /// <summary>
        /// Which service
        /// </summary>
        public string App { get; set; }

        /// <summary>
        /// Configuration options
        /// </summary>
        public object Config { get; set; }

        /// <summary>
        /// Build
        /// </summary>
        public Build Build { get; set; }
    }
}

