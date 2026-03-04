namespace ISPAdmin.Data.Entities;

/// <summary>
/// Many-to-many mapping between payment instruments and gateways.
/// Supports routing priority and default designation per instrument.
/// </summary>
public class PaymentInstrumentGateway : EntityBase
{
    /// <summary>
    /// Payment instrument identifier.
    /// </summary>
    public int PaymentInstrumentId { get; set; }

    /// <summary>
    /// Payment gateway identifier.
    /// </summary>
    public int PaymentGatewayId { get; set; }

    /// <summary>
    /// Whether this mapping is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this gateway is the preferred default for the instrument.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Lower value means higher routing priority.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Soft delete timestamp.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Navigation to payment instrument.
    /// </summary>
    public PaymentInstrument PaymentInstrument { get; set; } = null!;

    /// <summary>
    /// Navigation to payment gateway.
    /// </summary>
    public PaymentGateway PaymentGateway { get; set; } = null!;
}
