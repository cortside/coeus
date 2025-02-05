using System;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Facade;
using Cortside.Common.Logging;
using Cortside.DomainEvent;
using Cortside.DomainEvent.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            using (logger.PushProperty("OrderResourceId", @event.Data.OrderResourceId)) {
                logger.LogDebug("Handling {EventName} for ShoppingCart {OrderResourceId}", nameof(OrderStateChangedEvent), @event.Data.OrderResourceId);

                using (IServiceScope scope = serviceProvider.CreateScope()) {
                    var facade = scope.ServiceProvider.GetRequiredService<IOrderFacade>();

                    var dto = await facade.SendNotificationAsync(@event.Data.OrderResourceId).ConfigureAwait(false);
                    logger.LogInformation("Emailing customer at {Email} for change to order {OrderResourceId}", dto.Customer.Email, dto.OrderResourceId);
                    logger.LogInformation("order was observed changing it's state with body: {Body} and entity: {Entity}", JsonConvert.SerializeObject(@event.Data), JsonConvert.SerializeObject(dto));
                }

                logger.LogDebug("Successfully handled {EventName} for ShoppingCart {OrderResourceId}", nameof(OrderStateChangedEvent), @event.Data.OrderResourceId);
                return HandlerResult.Success;
            }
        }
    }
}
