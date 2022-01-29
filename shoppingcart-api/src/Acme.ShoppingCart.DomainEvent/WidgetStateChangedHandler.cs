using System;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.DomainService;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Handlers;
using Medallion.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;

namespace Acme.ShoppingCart.DomainEvent {
    /// <summary>
    /// Handles domain event <see cref="WidgetStageChangedEvent"/>
    /// </summary>
    public class WidgetStateChangedHandler : IDomainEventHandler<WidgetStageChangedEvent> {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<WidgetStateChangedHandler> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public WidgetStateChangedHandler(IServiceProvider serviceProvider, ILogger<WidgetStateChangedHandler> logger) {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task<HandlerResult> HandleAsync(DomainEventMessage<WidgetStageChangedEvent> @event) {
            using (LogContext.PushProperty("MessageId", @event.MessageId))
            using (LogContext.PushProperty("CorrelationId", @event.CorrelationId))
            using (LogContext.PushProperty("WidgetId", @event.Data.WidgetId)) {
                logger.LogDebug($"Handling {typeof(WidgetStageChangedEvent).Name} for WebApiStarter {@event.Data.WidgetId}");

                using (IServiceScope scope = serviceProvider.CreateScope()) {
                    var service = scope.ServiceProvider.GetRequiredService<IWidgetService>();
                    var lockProvider = scope.ServiceProvider.GetRequiredService<IDistributedLockProvider>();
                    var lockName = $"WidgetId:{@event.Data.WidgetId}";

                    logger.LogDebug($"Acquiring lock for {lockName}");
                    await using (await lockProvider.AcquireLockAsync(lockName).ConfigureAwait(false)) {
                        logger.LogDebug($"Acquired lock for {lockName}");
                        var entity = await service.GetWidgetAsync(@event.Data.WidgetId).ConfigureAwait(false);
                        // simulate more work with sleep
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        logger.LogInformation($"widget was observed changing it's state with body: {JsonConvert.SerializeObject(@event.Data)} and entity: {JsonConvert.SerializeObject(entity)}");
                    }
                }

                logger.LogDebug($"Successfully handled {typeof(WidgetStageChangedEvent).Name} for WebApiStarter {@event.Data.WidgetId}");
                return HandlerResult.Success;
            }
        }
    }
}
