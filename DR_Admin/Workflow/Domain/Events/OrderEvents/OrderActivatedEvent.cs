using ISPAdmin.Workflow.Domain.Events;

namespace ISPAdmin.Workflow.Domain.Events.OrderEvents;

/// <summary>
/// Event raised when an order is activated
/// </summary>
public class OrderActivatedEvent : DomainEventBase
{
    public override string EventType => "OrderActivated";
    
    public string OrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int? ServiceId { get; set; }
    public DateTime ActivatedAt { get; set; }
}
