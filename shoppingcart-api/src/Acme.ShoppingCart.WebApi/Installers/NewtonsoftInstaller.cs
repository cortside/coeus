#pragma warning disable S125 // Sections of code should not be commented out

using Cortside.Common.BootStrap;
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
                // intentionally commented out because of conflict with
                // Microsoft.AspNetCore.Mvc.Testing > 6.0.7 and Microsoft.NET.Test.Sdk > 17.2.0
                //settings.Converters.Add(new IsoTimeSpanConverter());
                return settings;
            };
        }
    }
}
