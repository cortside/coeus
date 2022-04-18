using Cortside.Common.BootStrap;
using Cortside.Common.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Acme.ShoppingCart.WebApi.Installers {
    public class NewtonsoftInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            JsonConvert.DefaultSettings = () => {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                settings.Converters.Add(new IsoDateTimeConverter {
                    DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"
                });
                settings.Converters.Add(new IsoTimeSpanConverter());
                return settings;
            };
        }
    }
}
