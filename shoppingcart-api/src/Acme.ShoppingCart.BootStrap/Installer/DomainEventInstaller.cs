using System;
using System.Collections.Generic;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.DomainEvent;
using Cortside.Common.BootStrap;
using Cortside.DomainEvent;
using Cortside.DomainEvent.EntityFramework;
using Cortside.DomainEvent.EntityFramework.Hosting;
using Cortside.DomainEvent.Handlers;
using Cortside.DomainEvent.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ShoppingCart.BootStrap.Installer {
    public class DomainEventInstaller : IInstaller {
        public void Install(IServiceCollection services, IConfigurationRoot configuration) {
            var config = configuration.GetSection("ServiceBus");
            var rsettings = new DomainEventReceiverSettings {
                Queue = config.GetValue<string>("Queue"),
                AppName = config.GetValue<string>("AppName"),
                Protocol = config.GetValue<string>("Protocol"),
                PolicyName = config.GetValue<string>("Policy"),
                Key = config.GetValue<string>("Key"),
                Namespace = config.GetValue<string>("Namespace"),
                Durable = 1,
                Credits = config.GetValue<int>("Credits")
            };
            services.AddSingleton(rsettings);

            var psettings = new DomainEventPublisherSettings {
                Topic = config.GetValue<string>("Topic"),
                AppName = config.GetValue<string>("AppName"),
                Protocol = config.GetValue<string>("Protocol"),
                PolicyName = config.GetValue<string>("Policy"),
                Key = config.GetValue<string>("Key"),
                Namespace = config.GetValue<string>("Namespace"),
                Durable = 1,
                Credits = config.GetValue<int>("Credits")
            };
            services.AddSingleton(psettings);

            // Register Hosted Services
            services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
            services.AddTransient<IDomainEventOutboxPublisher, DomainEventOutboxPublisher<DatabaseContext>>();
            services.AddTransient<IDomainEventHandler<CustomerStateChangedEvent>, WidgetStateChangedHandler>();
            services.AddSingleton<IDomainEventReceiver, DomainEventReceiver>();

            //services.AddSingleton<IDomainEventHandler<ContractorStateChangedEvent>, ContractorStateChangedEventHandler>();

            var receiverHostedServiceSettings = configuration.GetSection("ReceiverHostedService").Get<ReceiverHostedServiceSettings>();
            receiverHostedServiceSettings.MessageTypes = new Dictionary<string, Type> {
                { typeof(CustomerStateChangedEvent).FullName, typeof(CustomerStateChangedEvent) }
            };
            services.AddSingleton(receiverHostedServiceSettings);
            services.AddHostedService<ReceiverHostedService>();

            // outbox hosted service
            var outboxConfiguration = configuration.GetSection("OutboxHostedService").Get<OutboxHostedServiceConfiguration>();
            services.AddSingleton(outboxConfiguration);
            services.AddHostedService<OutboxHostedService<DatabaseContext>>();
        }
    }
}
