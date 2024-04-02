using System.Threading.Tasks;
using Acme.ShoppingCart.Configuration;
using Cortside.Common.Correlation;
using Cortside.Common.Hosting;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.Hosting {
    public class ExampleHostedService : TimedHostedService {
        public ExampleHostedService(ILogger<ExampleHostedService> logger, ExampleHostedServiceConfiguration config) : base(logger, config.Enabled, config.Interval, true) {
        }

        protected override Task ExecuteIntervalAsync() {
            var correlationId = CorrelationContext.GetCorrelationId();

            // run something async in async method
            return Task.Run(() => logger.LogInformation("CorrelationId: {CorrelationId}", correlationId));
        }
    }
}
