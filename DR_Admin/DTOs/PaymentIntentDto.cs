using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a payment intent
/// </summary>
public class PaymentIntentDto
{
    /// <summary>
    /// Unique identifier for the payment intent
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Invoice ID
    /// </summary>
    public int? InvoiceId { get; set; }

    /// <summary>
    /// Order ID
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Amount to be charged
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Current status
    /// </summary>
    public PaymentIntentStatus Status { get; set; }

    /// <summary>
    /// Payment gateway ID
    /// </summary>
    public int PaymentGatewayId { get; set; }

    /// <summary>
    /// Gateway payment intent ID
    /// </summary>
    public string GatewayIntentId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret for frontend
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Date authorized
    /// </summary>
    public DateTime? AuthorizedAt { get; set; }

    /// <summary>
    /// Date captured
    /// </summary>
    public DateTime? CapturedAt { get; set; }

    /// <summary>
    /// Date failed
    /// </summary>
    public DateTime? FailedAt { get; set; }

    /// <summary>
    /// Failure reason
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

    /// <summary>
    /// Date created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
