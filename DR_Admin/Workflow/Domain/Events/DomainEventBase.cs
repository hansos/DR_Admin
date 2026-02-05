namespace ISPAdmin.Workflow.Domain.Events;

/// <summary>
/// Base class for domain events with common properties
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public abstract string EventType { get; }
    public int AggregateId { get; set; }
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}
