namespace ISPAdmin.Domain.Events.InvoiceEvents;

/// <summary>
/// Event raised when an invoice becomes overdue
/// </summary>
public class InvoiceOverdueEvent : DomainEventBase
{
    public override string EventType => "InvoiceOverdue";
    
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public decimal AmountDue { get; set; }
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
}
