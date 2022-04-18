using Cortside.AspNetCore.ApplicationInsights.TelemetryInitializers;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Cortside.AspNetCore.ApplicationInsights
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCloudRoleNameInitializer(this IServiceCollection serviceCollection, string cloudRoleName)
        {
            return serviceCollection
                    .AddSingleton<ITelemetryInitializer>(
                            (_) => new CloudRoleNameTelemetryInitializer(cloudRoleName));
        }
    }
}
