namespace ISPAdmin.Data.Entities;

/// <summary>
/// Outbox pattern entity for reliable event delivery
/// Events are stored in the database within the same transaction as domain changes
/// then processed asynchronously by a background service
/// </summary>
public class OutboxEvent : EntityBase
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    public Guid EventId { get; set; }
    
    /// <summary>
    /// Type of the event (e.g., "DomainRegistered")
    /// </summary>
    public string EventType { get; set; } = string.Empty;
    
    /// <summary>
    /// Serialized event payload as JSON
    /// </summary>
    public string Payload { get; set; } = string.Empty;
    
    /// <summary>
    /// When the event occurred
    /// </summary>
    public DateTime OccurredAt { get; set; }
    
    /// <summary>
    /// When the event was successfully processed (null if not yet processed)
    /// </summary>
    public DateTime? ProcessedAt { get; set; }
    
    /// <summary>
    /// Number of processing attempts
    /// </summary>
    public int RetryCount { get; set; }
    
    /// <summary>
    /// Error message from last failed processing attempt
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Correlation ID for tracking related events
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;
}
