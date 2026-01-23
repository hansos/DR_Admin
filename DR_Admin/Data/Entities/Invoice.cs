using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

public class Invoice : EntityBase
{
    // Human-readable invoice number (legal identifier)
    public string InvoiceNumber { get; set; } = string.Empty;

    // Relationships
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    // Status & Lifecycle
    public InvoiceStatus Status { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }

    // Financials (Snapshot at issue time)
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }

    // Currency & Tax
    public string CurrencyCode { get; set; } = "EUR";
    public decimal TaxRate { get; set; }
    public string TaxName { get; set; } = "VAT";

    // Billing Snapshot (important!)
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public string CustomerTaxId { get; set; } = string.Empty;

    // Payment
    public string PaymentReference { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;

    // Notes & Metadata
    public string Notes { get; set; } = string.Empty;
    public string InternalComment { get; set; } = string.Empty;

    // Soft delete (rare but useful for drafts)
    public DateTime? DeletedAt { get; set; }

    // Navigation Properties
    public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
