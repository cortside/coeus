using System;
using System.Threading.Tasks;
using Cortside.DomainEvent;
using Cortside.DomainEvent.EntityFramework;

namespace Acme.IdentityServer.WebApi.IntegrationTests.Helpers {
    public class DomainEventPublisherStub : IDomainEventOutboxPublisher {
        public DomainEventError Error { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event PublisherClosedCallback Closed;

        public Task ScheduleMessageAsync<T>(T @event, DateTime scheduledEnqueueTimeUtc) where T : class {
            throw new NotImplementedException();
        }

        public Task ScheduleMessageAsync<T>(T @event, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class {
            throw new NotImplementedException();
        }

        public Task ScheduleMessageAsync<T>(T @event, string eventType, string address, string correlationId, DateTime scheduledEnqueueTimeUtc) where T : class {
            throw new NotImplementedException();
        }

        public Task ScheduleMessageAsync(string data, string eventType, string address, string correlationId, DateTime scheduledEnqueueTimeUtc) {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(T @event) where T : class {
            return Task.CompletedTask;
        }

        public Task SendAsync<T>(string eventType, string address, T @event) where T : class {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(string eventType, string address, T @event, string correlationId) where T : class {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(T @event, string correlationId) where T : class {
            throw new NotImplementedException();
        }

        public Task SendAsync(string eventType, string address, string data) {
            throw new NotImplementedException();
        }

        public Task SendAsync(string eventType, string address, string data, string correlationId) {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(T @event, string correlationId, string messageId) where T : class {
            throw new NotImplementedException();
        }

        public Task SendAsync<T>(T @event, string eventType, string address, string correlationId) where T : class {
            throw new NotImplementedException();
        }

        public Task SendAsync(string eventType, string address, string data, string correlationId, string messageId) {
            throw new NotImplementedException();
        }

        Task IDomainEventPublisher.PublishAsync<T>(T @event) {
            return Task.CompletedTask;
        }

        Task IDomainEventPublisher.PublishAsync<T>(T @event, string correlationId) {
            throw new NotImplementedException();
        }

        Task IDomainEventPublisher.PublishAsync<T>(T @event, EventProperties properties) {
            throw new NotImplementedException();
        }

        Task IDomainEventPublisher.PublishAsync(string body, EventProperties properties) {
            throw new NotImplementedException();
        }

        Task IDomainEventPublisher.ScheduleAsync<T>(T @event, DateTime scheduledEnqueueTimeUtc) {
            throw new NotImplementedException();
        }

        Task IDomainEventPublisher.ScheduleAsync<T>(T @event, DateTime scheduledEnqueueTimeUtc, string correlationId) {
            throw new NotImplementedException();
        }

        Task IDomainEventPublisher.ScheduleAsync<T>(T @event, DateTime scheduledEnqueueTimeUtc, EventProperties properties) {
            throw new NotImplementedException();
        }

        Task IDomainEventPublisher.ScheduleAsync(string body, DateTime scheduledEnqueueTimeUtc, EventProperties properties) {
            throw new NotImplementedException();
        }
    }
}
