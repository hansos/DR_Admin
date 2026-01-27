namespace ISPAdmin.Domain.Events.InvoiceEvents;

/// <summary>
/// Event raised when an invoice is generated
/// </summary>
public class InvoiceGeneratedEvent : DomainEventBase
{
    public override string EventType => "InvoiceGenerated";
    
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime DueDate { get; set; }
    public int? OrderId { get; set; }
}
