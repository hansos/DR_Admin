namespace ISPAdmin.Domain.Events.InvoiceEvents;

/// <summary>
/// Event raised when an invoice is paid
/// </summary>
public class InvoicePaidEvent : DomainEventBase
{
    public override string EventType => "InvoicePaid";
    
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaidAt { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public int? PaymentTransactionId { get; set; }
}
