using ISPAdmin.Workflow.Domain.Events;

namespace ISPAdmin.Workflow.Domain.Services;

/// <summary>
/// Interface for publishing domain events
/// </summary>
public interface IDomainEventPublisher
{
    /// <summary>
    /// Publishes a domain event (stores in outbox for async processing)
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent;
    
    /// <summary>
    /// Publishes multiple domain events in a batch
    /// </summary>
    Task PublishBatchAsync(IEnumerable<IDomainEvent> events);
}
