namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a payment gateway
/// </summary>
public class PaymentGatewayDto
{
    /// <summary>
    /// Unique identifier of the payment gateway
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the payment gateway
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Payment gateway provider code (stripe, paypal, square)
    /// </summary>
    public string ProviderCode { get; set; } = string.Empty;

    /// <summary>
    /// Whether this gateway is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether this is the default payment gateway
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// API key or access token (masked for security)
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Whether to use sandbox/test mode
    /// </summary>
    public bool UseSandbox { get; set; }

    /// <summary>
    /// Webhook URL for receiving payment notifications
    /// </summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>
    /// Display order for UI sorting
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Description of the payment gateway
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Logo URL or path
    /// </summary>
    public string LogoUrl { get; set; } = string.Empty;

    /// <summary>
    /// Supported currencies (comma-separated)
    /// </summary>
    public string SupportedCurrencies { get; set; } = string.Empty;

    /// <summary>
    /// Transaction fee percentage (if applicable)
    /// </summary>
    public decimal FeePercentage { get; set; }

    /// <summary>
    /// Fixed transaction fee amount (if applicable)
    /// </summary>
    public decimal FixedFee { get; set; }

    /// <summary>
    /// Notes or additional information
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Date and time when the record was soft deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new payment gateway
/// </summary>
public class CreatePaymentGatewayDto
{
    /// <summary>
    /// Name of the payment gateway
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Payment gateway provider code (stripe, paypal, square)
    /// </summary>
    public string ProviderCode { get; set; } = string.Empty;

    /// <summary>
    /// Whether this gateway is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this is the default payment gateway
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// API key or access token
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// API secret or client secret
    /// </summary>
    public string ApiSecret { get; set; } = string.Empty;

    /// <summary>
    /// Additional configuration settings in JSON format
    /// </summary>
    public string ConfigurationJson { get; set; } = string.Empty;

    /// <summary>
    /// Whether to use sandbox/test mode
    /// </summary>
    public bool UseSandbox { get; set; } = true;

    /// <summary>
    /// Webhook URL for receiving payment notifications
    /// </summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>
    /// Webhook secret for validating webhook requests
    /// </summary>
    public string WebhookSecret { get; set; } = string.Empty;

    /// <summary>
    /// Display order for UI sorting
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Description of the payment gateway
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Logo URL or path
    /// </summary>
    public string LogoUrl { get; set; } = string.Empty;

    /// <summary>
    /// Supported currencies (comma-separated)
    /// </summary>
    public string SupportedCurrencies { get; set; } = "USD,EUR,GBP";

    /// <summary>
    /// Transaction fee percentage (if applicable)
    /// </summary>
    public decimal FeePercentage { get; set; } = 0;

    /// <summary>
    /// Fixed transaction fee amount (if applicable)
    /// </summary>
    public decimal FixedFee { get; set; } = 0;

    /// <summary>
    /// Notes or additional information
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for updating an existing payment gateway
/// </summary>
public class UpdatePaymentGatewayDto
{
    /// <summary>
    /// Name of the payment gateway
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Payment gateway provider code (stripe, paypal, square)
    /// </summary>
    public string ProviderCode { get; set; } = string.Empty;

    /// <summary>
    /// Whether this gateway is currently active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether this is the default payment gateway
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// API key or access token (only update if provided)
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// API secret or client secret (only update if provided)
    /// </summary>
    public string? ApiSecret { get; set; }

    /// <summary>
    /// Additional configuration settings in JSON format
    /// </summary>
    public string ConfigurationJson { get; set; } = string.Empty;

    /// <summary>
    /// Whether to use sandbox/test mode
    /// </summary>
    public bool UseSandbox { get; set; }

    /// <summary>
    /// Webhook URL for receiving payment notifications
    /// </summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>
    /// Webhook secret for validating webhook requests (only update if provided)
    /// </summary>
    public string? WebhookSecret { get; set; }

    /// <summary>
    /// Display order for UI sorting
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Description of the payment gateway
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Logo URL or path
    /// </summary>
    public string LogoUrl { get; set; } = string.Empty;

    /// <summary>
    /// Supported currencies (comma-separated)
    /// </summary>
    public string SupportedCurrencies { get; set; } = string.Empty;

    /// <summary>
    /// Transaction fee percentage (if applicable)
    /// </summary>
    public decimal FeePercentage { get; set; }

    /// <summary>
    /// Fixed transaction fee amount (if applicable)
    /// </summary>
    public decimal FixedFee { get; set; }

    /// <summary>
    /// Notes or additional information
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}
