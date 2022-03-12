using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

internal class AppInsightsInitializer : ITelemetryInitializer {
    public void Initialize(ITelemetry telemetry) {

        // TODO: need to add shoppingcart to the replacements

        telemetry.Context.Cloud.RoleName = "shoppingcart-api";
    }
}
