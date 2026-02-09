namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a secure payment method token for recurring payments
/// </summary>
public class PaymentMethodToken : EntityBase
{
    /// <summary>
    /// Gets or sets the customer payment method ID
    /// </summary>
    public int CustomerPaymentMethodId { get; set; }

    /// <summary>
    /// Gets or sets the customer payment method navigation property
    /// </summary>
    public CustomerPaymentMethod CustomerPaymentMethod { get; set; } = null!;

    /// <summary>
    /// Gets or sets the encrypted token from payment gateway
    /// </summary>
    public string EncryptedToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the gateway-specific customer ID (e.g., Stripe Customer ID)
    /// </summary>
    public string GatewayCustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the gateway-specific payment method ID
    /// </summary>
    public string GatewayPaymentMethodId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token expiration date
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the last 4 digits of card for display
    /// </summary>
    public string Last4Digits { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the card brand (Visa, Mastercard, etc.)
    /// </summary>
    public string CardBrand { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the card expiry month
    /// </summary>
    public int? ExpiryMonth { get; set; }

    /// <summary>
    /// Gets or sets the card expiry year
    /// </summary>
    public int? ExpiryYear { get; set; }

    /// <summary>
    /// Gets or sets whether this is the default payment method
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets whether this token has been verified
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Gets or sets the verification date
    /// </summary>
    public DateTime? VerifiedAt { get; set; }
}
