using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Cortside.AspNetCore
{
    public class WarmupServicesStartupTask : IStartupTask
    {
        private readonly IServiceProvider provider;
        private readonly ILogger<WarmupServicesStartupTask> logger;
        public WarmupServicesStartupTask(ILogger<WarmupServicesStartupTask> logger, IServiceProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var timer = new Stopwatch();
            timer.Start();
            logger.LogInformation("starting warmup task");
            foreach (var singleton in GetSingletons(provider.GetRequiredService<IServiceCollection>()))
            {
                provider.GetServices(singleton);
            }
            timer.Stop();
            logger.LogInformation("warmup took {elapsedMilliseconds} ms", timer.ElapsedMilliseconds);
            return Task.CompletedTask;
        }

        static IEnumerable<Type> GetSingletons(IServiceCollection services)
        {
            return services
                .Where(descriptor =>
                    descriptor.Lifetime == ServiceLifetime.Singleton &&
                    descriptor.ImplementationType != typeof(WarmupServicesStartupTask) &&
                    !descriptor.ServiceType.ContainsGenericParameters)
                .Select(descriptor => descriptor.ServiceType)
                .Distinct();
        }
    }
}
