using System.Text.Json;
using ISPAdmin.Data;
using ISPAdmin.Data.Entities;
using ISPAdmin.Workflow.Domain.Events;

namespace ISPAdmin.Workflow.Domain.Services;

/// <summary>
/// Publishes domain events using the outbox pattern
/// Events are stored in the database and processed asynchronously
/// </summary>
public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly ApplicationDbContext _context;
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<DomainEventPublisher>();

    public DomainEventPublisher(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        try
        {
            _log.Information("Publishing event {EventType} for aggregate {AggregateId}", 
                @event.EventType, @event.AggregateId);

            var outboxEvent = new OutboxEvent
            {
                EventId = @event.EventId,
                EventType = @event.EventType,
                Payload = JsonSerializer.Serialize(@event),
                OccurredAt = @event.OccurredAt,
                CorrelationId = @event.CorrelationId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.OutboxEvents.Add(outboxEvent);
            await _context.SaveChangesAsync();

            _log.Information("Successfully published event {EventType} with ID {EventId}", 
                @event.EventType, @event.EventId);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Failed to publish event {EventType}", @event.EventType);
            throw;
        }
    }

    public async Task PublishBatchAsync(IEnumerable<IDomainEvent> events)
    {
        var eventList = events.ToList();
        
        if (!eventList.Any())
            return;

        try
        {
            _log.Information("Publishing batch of {Count} events", eventList.Count);

            var outboxEvents = eventList.Select(@event => new OutboxEvent
            {
                EventId = @event.EventId,
                EventType = @event.EventType,
                Payload = JsonSerializer.Serialize(@event),
                OccurredAt = @event.OccurredAt,
                CorrelationId = @event.CorrelationId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            _context.OutboxEvents.AddRange(outboxEvents);
            await _context.SaveChangesAsync();

            _log.Information("Successfully published batch of {Count} events", eventList.Count);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Failed to publish event batch");
            throw;
        }
    }
}
