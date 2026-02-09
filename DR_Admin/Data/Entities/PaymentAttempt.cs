using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a payment attempt for an invoice
/// </summary>
public class PaymentAttempt : EntityBase
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
    /// Gets or sets the payment transaction ID (null if attempt failed)
    /// </summary>
    public int? PaymentTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the payment transaction navigation property
    /// </summary>
    public PaymentTransaction? PaymentTransaction { get; set; }

    /// <summary>
    /// Gets or sets the customer payment method ID
    /// </summary>
    public int CustomerPaymentMethodId { get; set; }

    /// <summary>
    /// Gets or sets the customer payment method navigation property
    /// </summary>
    public CustomerPaymentMethod CustomerPaymentMethod { get; set; } = null!;

    /// <summary>
    /// Gets or sets the attempted amount
    /// </summary>
    public decimal AttemptedAmount { get; set; }

    /// <summary>
    /// Gets or sets the currency code
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets the payment attempt status
    /// </summary>
    public PaymentAttemptStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the gateway response
    /// </summary>
    public string GatewayResponse { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the gateway transaction ID
    /// </summary>
    public string GatewayTransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error code from gateway
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the retry count
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Gets or sets the next retry date
    /// </summary>
    public DateTime? NextRetryAt { get; set; }

    /// <summary>
    /// Gets or sets whether authentication is required
    /// </summary>
    public bool RequiresAuthentication { get; set; }

    /// <summary>
    /// Gets or sets the authentication URL for 3D Secure
    /// </summary>
    public string AuthenticationUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authentication status
    /// </summary>
    public AuthenticationStatus AuthenticationStatus { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the payment attempt
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user agent of the payment attempt
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;
}
