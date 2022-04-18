using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Cortside.AspNetCore.ApplicationInsights.TelemetryInitializers
{
    public class CloudRoleNameTelemetryInitializer : ITelemetryInitializer
    {
        private readonly string cloudRoleName;

        public CloudRoleNameTelemetryInitializer(string cloudRoleName)
        {
            this.cloudRoleName = cloudRoleName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = cloudRoleName;
        }
    }
}
