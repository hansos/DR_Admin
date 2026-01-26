using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a customer payment method
/// </summary>
public class CustomerPaymentMethodDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Payment gateway ID
    /// </summary>
    public int PaymentGatewayId { get; set; }

    /// <summary>
    /// Payment gateway name
    /// </summary>
    public string PaymentGatewayName { get; set; } = string.Empty;

    /// <summary>
    /// Payment method type
    /// </summary>
    public PaymentMethodType Type { get; set; }

    /// <summary>
    /// Last 4 digits
    /// </summary>
    public string Last4Digits { get; set; } = string.Empty;

    /// <summary>
    /// Expiry month
    /// </summary>
    public int? ExpiryMonth { get; set; }

    /// <summary>
    /// Expiry year
    /// </summary>
    public int? ExpiryYear { get; set; }

    /// <summary>
    /// Card brand
    /// </summary>
    public string CardBrand { get; set; } = string.Empty;

    /// <summary>
    /// Cardholder name
    /// </summary>
    public string CardholderName { get; set; } = string.Empty;

    /// <summary>
    /// Is default payment method
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Is verified
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Date created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
