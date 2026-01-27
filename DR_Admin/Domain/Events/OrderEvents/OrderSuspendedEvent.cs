namespace ISPAdmin.Domain.Events.OrderEvents;

/// <summary>
/// Event raised when an order is suspended
/// </summary>
public class OrderSuspendedEvent : DomainEventBase
{
    public override string EventType => "OrderSuspended";
    
    public string OrderNumber { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? SuspendedBy { get; set; }
}
