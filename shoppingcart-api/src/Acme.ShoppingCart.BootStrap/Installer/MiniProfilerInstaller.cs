using Cortside.Common.BootStrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class MiniProfilerInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfiguration configuration) {
            services.AddMiniProfiler(options => {
                options.RouteBasePath = "/profiler";

                // Control which SQL formatter to use, InlineFormatter is the default
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
                options.ColorScheme = StackExchange.Profiling.ColorScheme.Auto;

                // Enabled sending the Server-Timing header on responses
                options.EnableServerTimingHeader = true;
            }).AddEntityFramework();
        }
    }
}
