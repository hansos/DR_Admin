namespace ISPAdmin.Domain.Events.DomainEvents;

/// <summary>
/// Event raised when a domain is suspended
/// </summary>
public class DomainSuspendedEvent : DomainEventBase
{
    public override string EventType => "DomainSuspended";
    
    public string DomainName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? SuspendedBy { get; set; }
}
