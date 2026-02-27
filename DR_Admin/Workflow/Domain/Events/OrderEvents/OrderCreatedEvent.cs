using ISPAdmin.Data.Enums;
using ISPAdmin.Workflow.Domain.Events;

namespace ISPAdmin.Workflow.Domain.Events.OrderEvents;

/// <summary>
/// Event raised when a new order is created
/// </summary>
public class OrderCreatedEvent : DomainEventBase
{
    public override string EventType => "OrderCreated";
    
    public string OrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int? ServiceId { get; set; }
    public int? InvoiceId { get; set; }
    public OrderType OrderType { get; set; }
    public decimal TotalAmount { get; set; }
}
