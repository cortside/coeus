using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

internal class AppInsightsInitializer : ITelemetryInitializer {
    public void Initialize(ITelemetry telemetry) {
        telemetry.Context.Cloud.RoleName = "WebApiStarter";
    }
}
