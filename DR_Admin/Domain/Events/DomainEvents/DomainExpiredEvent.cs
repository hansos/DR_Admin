namespace ISPAdmin.Domain.Events.DomainEvents;

/// <summary>
/// Event raised when a domain expires
/// </summary>
public class DomainExpiredEvent : DomainEventBase
{
    public override string EventType => "DomainExpired";
    
    public string DomainName { get; set; } = string.Empty;
    public DateTime ExpiredAt { get; set; }
    public int CustomerId { get; set; }
    public bool AutoRenewEnabled { get; set; }
}
