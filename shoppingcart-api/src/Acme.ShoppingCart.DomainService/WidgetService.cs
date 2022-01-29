using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acme.DomainEvent.Events;
using Acme.ShoppingCart.Data;
using Acme.ShoppingCart.Dto;
using Cortside.DomainEvent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acme.ShoppingCart.DomainService {
    public class WidgetService : IWidgetService {
        private readonly DatabaseContext db;
        private readonly IDomainEventOutboxPublisher publisher;
        private readonly ILogger<WidgetService> logger;

        public WidgetService(DatabaseContext db, IDomainEventOutboxPublisher publisher, ILogger<WidgetService> logger) {
            this.db = db;
            this.publisher = publisher;
            this.logger = logger;
        }

        public async Task<WidgetDto> CreateWidgetAsync(WidgetDto dto) {
            var entity = new Domain.Widget() {
                Text = dto.Text,
                Width = dto.Width,
                Height = dto.Height
            };

            // user initiated transaction with retry strategy set needs to execute in new strategy 
            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {
                // need a transaction and 2 savechanges so that I have the id for the widget in the event
                using (var tx = await db.Database.BeginTransactionAsync().ConfigureAwait(false)) {
                    try {
                        db.Widgets.Add(entity);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                        var @event = new WidgetStageChangedEvent() { WidgetId = entity.WidgetId, Text = entity.Text, Width = entity.Width, Height = entity.Height, Timestamp = DateTime.UtcNow };
                        await publisher.PublishAsync(@event).ConfigureAwait(false);
                        await db.SaveChangesAsync().ConfigureAwait(false);
                        await tx.CommitAsync().ConfigureAwait(false);
                    } catch (Exception ex) {
                        logger.LogError(ex, "unhandled exception");
                        await tx.RollbackAsync().ConfigureAwait(false);
                        throw;
                    }
                }
            }).ConfigureAwait(false);

            return ToWidgetDto(entity);
        }

        public Task<WidgetDto> DeleteWidgetAsync(int widgetId) {
            throw new NotImplementedException();
        }

        public async Task<WidgetDto> GetWidgetAsync(int widgetId) {
            var entity = await db.Widgets.SingleAsync(x => x.WidgetId == widgetId).ConfigureAwait(false);
            return ToWidgetDto(entity);
        }

        public async Task<List<WidgetDto>> GetWidgetsAsync() {
            var entities = await db.Widgets.ToListAsync().ConfigureAwait(false);

            var dtos = new List<WidgetDto>();
            foreach (var entity in entities) {
                dtos.Add(ToWidgetDto(entity));
            }

            return dtos;
        }

        public async Task<WidgetDto> UpdateWidgetAsync(WidgetDto dto) {
            var entity = await db.Widgets.FirstOrDefaultAsync(w => w.WidgetId == dto.WidgetId).ConfigureAwait(false);
            entity.Text = dto.Text;
            entity.Width = dto.Width;
            entity.Height = dto.Height;

            var @event = new WidgetStageChangedEvent() { WidgetId = entity.WidgetId, Text = entity.Text, Width = entity.Width, Height = entity.Height, Timestamp = DateTime.UtcNow };
            await publisher.PublishAsync(@event).ConfigureAwait(false);

            await db.SaveChangesAsync().ConfigureAwait(false);
            return ToWidgetDto(entity);
        }

        public async Task PublishWidgetStateChangedEventAsync(int id) {
            var entity = await db.Widgets.FirstOrDefaultAsync(w => w.WidgetId == id).ConfigureAwait(false);

            var @event = new WidgetStageChangedEvent() { WidgetId = entity.WidgetId, Text = entity.Text, Width = entity.Width, Height = entity.Height, Timestamp = DateTime.UtcNow };
            await publisher.PublishAsync(@event).ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);
        }

        private WidgetDto ToWidgetDto(Domain.Widget entity) {
            return new WidgetDto() {
                WidgetId = entity.WidgetId,
                Text = entity.Text,
                Width = entity.Width,
                Height = entity.Height
            };
        }
    }
}
