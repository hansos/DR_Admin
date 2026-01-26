using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a stored payment method for a customer
/// </summary>
public class CustomerPaymentMethod : EntityBase
{
    /// <summary>
    /// Foreign key to the customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Foreign key to the payment gateway
    /// </summary>
    public int PaymentGatewayId { get; set; }

    /// <summary>
    /// Type of payment method
    /// </summary>
    public PaymentMethodType Type { get; set; }

    /// <summary>
    /// Tokenized payment method identifier from the gateway
    /// </summary>
    public string PaymentMethodToken { get; set; } = string.Empty;

    /// <summary>
    /// Last 4 digits of card or account number (for display purposes)
    /// </summary>
    public string Last4Digits { get; set; } = string.Empty;

    /// <summary>
    /// Expiry month for credit cards (1-12)
    /// </summary>
    public int? ExpiryMonth { get; set; }

    /// <summary>
    /// Expiry year for credit cards (e.g., 2025)
    /// </summary>
    public int? ExpiryYear { get; set; }

    /// <summary>
    /// Card brand (e.g., "Visa", "MasterCard", "Amex")
    /// </summary>
    public string CardBrand { get; set; } = string.Empty;

    /// <summary>
    /// Cardholder name
    /// </summary>
    public string CardholderName { get; set; } = string.Empty;

    /// <summary>
    /// Billing address in JSON format
    /// </summary>
    public string BillingAddressJson { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is the default payment method for the customer
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Whether this payment method is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this payment method has been verified
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Date when this payment method was verified
    /// </summary>
    public DateTime? VerifiedAt { get; set; }

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Navigation property to the customer
    /// </summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>
    /// Navigation property to the payment gateway
    /// </summary>
    public PaymentGateway PaymentGateway { get; set; } = null!;
}
