namespace ISPAdmin.Domain.Events.DomainEvents;

/// <summary>
/// Event raised when a domain is successfully registered
/// </summary>
public class DomainRegisteredEvent : DomainEventBase
{
    public override string EventType => "DomainRegistered";
    
    public string DomainName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int RegistrarId { get; set; }
    public DateTime ExpirationDate { get; set; }
    public decimal RegistrationPrice { get; set; }
    public bool AutoRenew { get; set; }
}
