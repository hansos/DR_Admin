using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a payment transaction for an invoice
/// </summary>
public class PaymentTransaction : EntityBase
{
    /// <summary>
    /// Foreign key to the invoice
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Foreign key to the payment intent
    /// </summary>
    public int? PaymentIntentId { get; set; }

    /// <summary>
    /// Payment method used (e.g., "Credit Card", "Bank Transfer")
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the payment transaction
    /// </summary>
    public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Pending;

    /// <summary>
    /// Transaction ID from the payment gateway
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Amount paid
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";
    
    /// <summary>
    /// Base currency for accounting (if different from CurrencyCode)
    /// </summary>
    public string? BaseCurrencyCode { get; set; }
    
    /// <summary>
    /// Exchange rate at time of payment (from base to payment currency)
    /// Null if base and payment currencies are the same
    /// </summary>
    public decimal? ExchangeRate { get; set; }
    
    /// <summary>
    /// Amount in base currency for accounting
    /// Null if base and payment currencies are the same
    /// </summary>
    public decimal? BaseAmount { get; set; }

    /// <summary>
    /// Foreign key to the payment gateway
    /// </summary>
    public int? PaymentGatewayId { get; set; }

    /// <summary>
    /// Raw response from the payment gateway in JSON format
    /// </summary>
    public string GatewayResponse { get; set; } = string.Empty;

    /// <summary>
    /// Reason for failure (if applicable)
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

    /// <summary>
    /// Date when the payment was processed
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Amount refunded from this transaction
    /// </summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>
    /// Whether this was an automatic payment
    /// </summary>
    public bool IsAutomatic { get; set; }

    /// <summary>
    /// Internal notes
    /// </summary>
    public string InternalNotes { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the invoice
    /// </summary>
    public Invoice Invoice { get; set; } = null!;

    /// <summary>
    /// Navigation property to the payment gateway
    /// </summary>
    public PaymentGateway? PaymentGateway { get; set; }

    /// <summary>
    /// Navigation property to the payment intent
    /// </summary>
    public PaymentIntent? PaymentIntent { get; set; }

    /// <summary>
    /// Collection of refunds for this transaction
    /// </summary>
    public ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    /// <summary>
    /// Collection of credit transactions related to this payment
    /// </summary>
    public ICollection<CreditTransaction> CreditTransactions { get; set; } = new List<CreditTransaction>();
}
