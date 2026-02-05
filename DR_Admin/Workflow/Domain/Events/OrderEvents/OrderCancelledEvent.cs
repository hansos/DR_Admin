using ISPAdmin.Workflow.Domain.Events;

namespace ISPAdmin.Workflow.Domain.Events.OrderEvents;

/// <summary>
/// Event raised when an order is cancelled
/// </summary>
public class OrderCancelledEvent : DomainEventBase
{
    public override string EventType => "OrderCancelled";
    
    public string OrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? CancelledBy { get; set; }
    public DateTime CancelledAt { get; set; }
}
