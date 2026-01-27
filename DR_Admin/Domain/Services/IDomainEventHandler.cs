using ISPAdmin.Domain.Events;

namespace ISPAdmin.Domain.Services;

/// <summary>
/// Base interface for domain event handlers
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles a domain event
    /// </summary>
    Task HandleAsync(TEvent @event);
}
