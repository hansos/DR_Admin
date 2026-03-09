namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a line item in an order
/// </summary>
public class OrderLine : EntityBase
{
    /// <summary>
    /// Foreign key to parent order
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Optional foreign key to service
    /// </summary>
    public int? ServiceId { get; set; }

    /// <summary>
    /// Line number for ordering in UI/documents
    /// </summary>
    public int LineNumber { get; set; } = 1;

    /// <summary>
    /// Descriptive line text
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Quantity for this line
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Unit price for this line
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total price for this line
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Indicates whether this line is recurring
    /// </summary>
    public bool IsRecurring { get; set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Navigation to parent order
    /// </summary>
    public Order Order { get; set; } = null!;

    /// <summary>
    /// Navigation to service when linked
    /// </summary>
    public Service? Service { get; set; }

    /// <summary>
    /// Sold hosting package instances linked to this order line
    /// </summary>
    public ICollection<SoldHostingPackage> SoldHostingPackages { get; set; } = new List<SoldHostingPackage>();

    /// <summary>
    /// Sold optional service instances linked to this order line
    /// </summary>
    public ICollection<SoldOptionalService> SoldOptionalServices { get; set; } = new List<SoldOptionalService>();
}
