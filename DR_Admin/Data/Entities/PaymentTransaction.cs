namespace ISPAdmin.Data.Entities;

public class PaymentTransaction : EntityBase
{
    public int InvoiceId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int? PaymentGatewayId { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public PaymentGateway? PaymentGateway { get; set; }
}
