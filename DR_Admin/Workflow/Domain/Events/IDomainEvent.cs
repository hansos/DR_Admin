namespace ISPAdmin.Workflow.Domain.Events;

/// <summary>
/// Base interface for all domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for this event instance
    /// </summary>
    Guid EventId { get; }
    
    /// <summary>
    /// When the event occurred
    /// </summary>
    DateTime OccurredAt { get; }
    
    /// <summary>
    /// Type of the event (e.g., "DomainRegistered")
    /// </summary>
    string EventType { get; }
    
    /// <summary>
    /// ID of the aggregate root this event relates to
    /// </summary>
    int AggregateId { get; }
    
    /// <summary>
    /// Correlation ID for tracking related events across workflows
    /// </summary>
    string CorrelationId { get; }
}
