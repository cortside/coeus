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
    /// Handles domain event <see cref="OrderStateChangedEvent"/>
    /// </summary>
    public class OrderStateChangedHandler : IDomainEventHandler<OrderStateChangedEvent> {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<OrderStateChangedHandler> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public OrderStateChangedHandler(IServiceProvider serviceProvider, ILogger<OrderStateChangedHandler> logger) {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task<HandlerResult> HandleAsync(DomainEventMessage<OrderStateChangedEvent> @event) {
            using (LogContext.PushProperty("MessageId", @event.MessageId))
            using (LogContext.PushProperty("CorrelationId", @event.CorrelationId))
            using (LogContext.PushProperty("OrderResourceId", @event.Data.OrderResourceId)) {
                logger.LogDebug($"Handling {typeof(OrderStateChangedEvent).Name} for ShoppingCart {@event.Data.OrderResourceId}");

                using (IServiceScope scope = serviceProvider.CreateScope()) {
                    var service = scope.ServiceProvider.GetRequiredService<IOrderService>();
                    var lockProvider = scope.ServiceProvider.GetRequiredService<IDistributedLockProvider>();
                    var lockName = $"OrderResourceId:{@event.Data.OrderResourceId}";

                    logger.LogDebug($"Acquiring lock for {lockName}");
                    await using (await lockProvider.AcquireLockAsync(lockName).ConfigureAwait(false)) {
                        logger.LogDebug($"Acquired lock for {lockName}");
                        var entity = await service.GetOrderAsync(@event.Data.OrderResourceId).ConfigureAwait(false);
                        logger.LogInformation($"Emailing customer at {entity.Customer.Email} for order change");

                        // simulate work
                        await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

                        logger.LogInformation($"order was observed changing it's state with body: {JsonConvert.SerializeObject(@event.Data)} and entity: {JsonConvert.SerializeObject(entity)}");
                    }
                }

                logger.LogDebug($"Successfully handled {typeof(OrderStateChangedEvent).Name} for ShoppingCart {@event.Data.OrderResourceId}");
                return HandlerResult.Success;
            }
        }
    }
}
