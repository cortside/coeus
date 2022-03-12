using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Acme.ShoppingCart.WebApi.Utils {
    internal class AppInsightsInitializer : ITelemetryInitializer {
        public void Initialize(ITelemetry telemetry) {
            telemetry.Context.Cloud.RoleName = "shoppingcart-api";
        }
    }
}
