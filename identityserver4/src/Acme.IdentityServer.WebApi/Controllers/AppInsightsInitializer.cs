using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Acme.IdentityServer.WebApi.Controllers {
    internal class AppInsightsInitializer : ITelemetryInitializer {
        public void Initialize(ITelemetry telemetry) {
            telemetry.Context.Cloud.RoleName = "identityserver";
        }
    }
}
