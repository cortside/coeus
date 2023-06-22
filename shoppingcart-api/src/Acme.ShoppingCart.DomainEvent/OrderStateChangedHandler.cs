using System;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Facade;
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
                logger.LogDebug("Handling {EventName} for ShoppingCart {OrderResourceId}", typeof(OrderStateChangedEvent).Name, @event.Data.OrderResourceId);

                using (IServiceScope scope = serviceProvider.CreateScope()) {
                    var facade = scope.ServiceProvider.GetRequiredService<IOrderFacade>();
                    var lockProvider = scope.ServiceProvider.GetRequiredService<IDistributedLockProvider>();
                    var lockName = $"OrderResourceId:{@event.Data.OrderResourceId}";

                    logger.LogDebug("Acquiring lock for {LockName}", lockName);
                    await using (await lockProvider.AcquireLockAsync(lockName).ConfigureAwait(false)) {
                        logger.LogDebug("Acquired lock for {LockName}", lockName);
                        var entity = await facade.SendNotificationAsync(@event.Data.OrderResourceId).ConfigureAwait(false);
                        logger.LogInformation("Emailing customer at {Email} for change to order {OrderResourceId}", entity.Customer.Email, entity.OrderResourceId);
                        logger.LogDebug("Handling change event for order {@order}", entity);
                        logger.LogInformation("order was observed changing it's state with body: {body} and entity: {entity}", JsonConvert.SerializeObject(@event.Data), JsonConvert.SerializeObject(entity));
                    }
                }

                logger.LogDebug("Successfully handled {EventName} for ShoppingCart {OrderResourceId}", nameof(OrderStateChangedEvent), @event.Data.OrderResourceId);
                return HandlerResult.Success;
            }
        }
    }
}
