namespace ISPAdmin.Domain.Events.DomainEvents;

/// <summary>
/// Event raised when a domain is transferred in or out
/// </summary>
public class DomainTransferredEvent : DomainEventBase
{
    public override string EventType => "DomainTransferred";
    
    public string DomainName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public bool IsTransferIn { get; set; }
    public int? PreviousRegistrarId { get; set; }
    public int? NewRegistrarId { get; set; }
    public DateTime TransferCompletedAt { get; set; }
}
