namespace ISPAdmin.Domain.Events.DomainEvents;

/// <summary>
/// Event raised when a domain is renewed
/// </summary>
public class DomainRenewedEvent : DomainEventBase
{
    public override string EventType => "DomainRenewed";
    
    public string DomainName { get; set; } = string.Empty;
    public DateTime PreviousExpirationDate { get; set; }
    public DateTime NewExpirationDate { get; set; }
    public decimal RenewalPrice { get; set; }
    public int RenewalYears { get; set; }
}
