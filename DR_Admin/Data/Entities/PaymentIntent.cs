using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a payment intent created before processing a payment
/// </summary>
public class PaymentIntent : EntityBase
{
    /// <summary>
    /// Foreign key to the invoice being paid
    /// </summary>
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Foreign key to the order (for new orders without invoice yet)
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Foreign key to the customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Amount to be charged
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code (e.g., "USD", "EUR")
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Current status of the payment intent
    /// </summary>
    public PaymentIntentStatus Status { get; set; } = PaymentIntentStatus.Created;

    /// <summary>
    /// Foreign key to the payment gateway
    /// </summary>
    public int PaymentGatewayId { get; set; }

    /// <summary>
    /// Payment intent ID from the gateway (e.g., Stripe PaymentIntent ID)
    /// </summary>
    public string GatewayIntentId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret for frontend payment UI
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// URL to return to after successful payment
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL to return to if payment is cancelled
    /// </summary>
    public string CancelUrl { get; set; } = string.Empty;

    /// <summary>
    /// Description of the payment
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string MetadataJson { get; set; } = string.Empty;

    /// <summary>
    /// Date when the payment was authorized
    /// </summary>
    public DateTime? AuthorizedAt { get; set; }

    /// <summary>
    /// Date when the payment was captured
    /// </summary>
    public DateTime? CapturedAt { get; set; }

    /// <summary>
    /// Date when the payment failed
    /// </summary>
    public DateTime? FailedAt { get; set; }

    /// <summary>
    /// Reason for failure (if applicable)
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

    /// <summary>
    /// Raw response from the payment gateway
    /// </summary>
    public string GatewayResponse { get; set; } = string.Empty;

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Navigation property to the invoice
    /// </summary>
    public Invoice? Invoice { get; set; }

    /// <summary>
    /// Navigation property to the order
    /// </summary>
    public Order? Order { get; set; }

    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Navigation property to the payment gateway
    /// </summary>
    public PaymentGateway PaymentGateway { get; set; } = null!;

    /// <summary>
    /// Navigation property to the resulting payment transaction
    /// </summary>
    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
