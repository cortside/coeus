//using System;
//using System.Threading.Tasks;
//using Cortside.Common.Correlation;
//using Cortside.DomainEvent;
//using Cortside.DomainEvent.Handlers;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Serilog.Context;

//namespace Acme.ShoppingCart.WebApi.Events {
//    /// <summary>
//    /// Handles domain event <see cref="ContractorStateChangedEvent"/>
//    /// </summary>
//    public class ContractorStateChangedEventHandler : IDomainEventHandler<ContractorStateChangedEvent> {
//        private readonly ILogger<ContractorStateChangedEventHandler> logger;
//        private readonly IServiceProvider serviceProvider;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ContractorStateChangedEventHandler"/> class.
//        /// </summary>
//        /// <param name="logger">Logger</param>
//        /// <param name="serviceProvider">Service Provider</param>
//        public ContractorStateChangedEventHandler(
//            ILogger<ContractorStateChangedEventHandler> logger,
//            IServiceProvider serviceProvider) {
//            this.logger = logger;
//            this.serviceProvider = serviceProvider;
//        }

//        /// <summary>
//        /// Handle with domainevent messages
//        /// </summary>
//        /// <param name="event"></param>
//        /// <returns></returns>
//        public async Task<HandlerResult> HandleAsync(DomainEventMessage<ContractorStateChangedEvent> @event) {
//            logger.LogInformation("Received ContractorStateChangedEvent {@contractorStateChangedEvent}", @event);
//            var correlationId = string.IsNullOrWhiteSpace(@event.CorrelationId) ? Guid.NewGuid().ToString() : @event.CorrelationId;
//            CorrelationContext.SetCorrelationId(correlationId);
//            using (LogContext.PushProperty("MessageId", @event.MessageId))
//            using (LogContext.PushProperty("CorrelationId", CorrelationContext.GetCorrelationId()))
//            using (LogContext.PushProperty("ContractorId", @event.Data.ContractorId)) {
//                using (var scope = serviceProvider.CreateScope()) {
//                    IContractorService contractorService = scope.ServiceProvider.GetRequiredService<IContractorService>();
//                    ContractorDto contractorDto = new ContractorDto {
//                        ContractorId = @event.Data.ContractorId,
//                        ContractorName = @event.Data.ContractorName,
//                        ContractorNumber = @event.Data.ContractorNumber,
//                        SponsorId = @event.Data.SponsorId,
//                        SponsorNumber = @event.Data.SponsorNumber
//                    };
//                    await contractorService.SyncContractorAsync(contractorDto).ConfigureAwait(false);
//                    return HandlerResult.Success;
//                }
//            }
//        }
//    }
//}
