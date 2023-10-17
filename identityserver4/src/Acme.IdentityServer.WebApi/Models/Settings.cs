using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Acme.IdentityServer.WebApi.Models {
    public class Settings {
        public string Deployment { get; set; }
        public string App { get; set; }
        public ConfigSettings Config { get; set; }
        public Build Build { get; set; }
    }

    public class ConfigSettings {
        public string Namespace { get; internal set; }
        public string Policy { get; internal set; }
    }

    /// <summary>
    /// Build information
    /// </summary>
    public class Build {
        /// <summary>
        /// Build date
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Build version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Build tag
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Suffix
        /// </summary>
        public string Suffix { get; set; }
    }
}
