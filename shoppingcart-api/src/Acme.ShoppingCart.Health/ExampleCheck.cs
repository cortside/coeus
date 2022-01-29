using System;
using System.Threading.Tasks;
using Cortside.Health;
using Cortside.Health.Checks;
using Cortside.Health.Enums;
using Cortside.Health.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.Health {
    public class ExampleCheck : Check {
        public ExampleCheck(IMemoryCache cache, ILogger<Check> logger, IAvailabilityRecorder recorder) : base(cache, logger, recorder) {
        }

        public override async Task<ServiceStatusModel> ExecuteAsync() {
            // add custom logic here
            var model = new ServiceStatusModel() {
                Healthy = true,
                Status = ServiceStatus.Ok,
                StatusDetail = "Example detail",
                Timestamp = DateTime.UtcNow
            };

            return await Task.FromResult<ServiceStatusModel>(model).ConfigureAwait(false);
        }
    }
}
