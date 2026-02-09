namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a partial payment applied to an invoice
/// </summary>
public class InvoicePayment : EntityBase
{
    /// <summary>
    /// Gets or sets the invoice ID
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Gets or sets the invoice navigation property
    /// </summary>
    public Invoice Invoice { get; set; } = null!;

    /// <summary>
    /// Gets or sets the payment transaction ID
    /// </summary>
    public int PaymentTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the payment transaction navigation property
    /// </summary>
    public PaymentTransaction PaymentTransaction { get; set; } = null!;

    /// <summary>
    /// Gets or sets the amount applied to this invoice
    /// </summary>
    public decimal AmountApplied { get; set; }

    /// <summary>
    /// Gets or sets the currency code
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets the invoice balance after this payment
    /// </summary>
    public decimal InvoiceBalance { get; set; }

    /// <summary>
    /// Gets or sets the invoice total amount
    /// </summary>
    public decimal InvoiceTotalAmount { get; set; }

    /// <summary>
    /// Gets or sets whether this payment fully paid the invoice
    /// </summary>
    public bool IsFullPayment { get; set; }
}
