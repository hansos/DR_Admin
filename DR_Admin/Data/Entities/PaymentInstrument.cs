namespace ISPAdmin.Data.Entities;

/// <summary>
/// Payment instrument catalog entry used to group gateways (e.g., CreditCard, PayPal, Cash)
/// </summary>
public class PaymentInstrument : EntityBase
{
    /// <summary>
    /// Stable instrument code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Display name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Normalized code for case-insensitive lookup
    /// </summary>
    public string NormalizedCode { get; set; } = string.Empty;

    /// <summary>
    /// Normalized name for case-insensitive lookup
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Optional description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Whether this instrument is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Display order for UI sorting
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Default payment gateway used for this instrument
    /// </summary>
    public int? DefaultGatewayId { get; set; }

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gateways associated with this instrument
    /// </summary>
    public ICollection<PaymentGateway> PaymentGateways { get; set; } = new List<PaymentGateway>();

    /// <summary>
    /// Default gateway navigation
    /// </summary>
    public PaymentGateway? DefaultGateway { get; set; }
}
