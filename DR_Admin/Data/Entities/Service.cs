namespace ISPAdmin.Data.Entities;

/// <summary>
/// Represents a service offering (hosting plan, domain registration, etc.)
/// </summary>
public class Service : EntityBase
{
    /// <summary>
    /// Name of the service
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the service
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the service type
    /// </summary>
    public int ServiceTypeId { get; set; }

    /// <summary>
    /// Foreign key to the billing cycle
    /// </summary>
    public int BillingCycleId { get; set; }

    /// <summary>
    /// Recurring price per billing cycle
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// One-time setup fee
    /// </summary>
    public decimal SetupFee { get; set; }

    /// <summary>
    /// Number of days for trial period (0 = no trial)
    /// </summary>
    public int TrialDays { get; set; }

    /// <summary>
    /// Whether this service is currently active and available for purchase
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this service is featured/promoted
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// SKU or product code
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the reseller company (if applicable)
    /// </summary>
    public int? ResellerCompanyId { get; set; }

    /// <summary>
    /// Foreign key to the sales agent (if applicable)
    /// </summary>
    public int? SalesAgentId { get; set; }

    /// <summary>
    /// Maximum quantity that can be ordered
    /// </summary>
    public int? MaxQuantity { get; set; }

    /// <summary>
    /// Minimum quantity required for order
    /// </summary>
    public int MinQuantity { get; set; } = 1;

    /// <summary>
    /// Sort order for display
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Service specifications in JSON format
    /// </summary>
    public string SpecificationsJson { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the service type
    /// </summary>
    public ServiceType ServiceType { get; set; } = null!;

    /// <summary>
    /// Navigation property to the billing cycle
    /// </summary>
    public BillingCycle BillingCycle { get; set; } = null!;

    /// <summary>
    /// Navigation property to the reseller company
    /// </summary>
    public ResellerCompany? ResellerCompany { get; set; }

    /// <summary>
    /// Navigation property to the sales agent
    /// </summary>
    public SalesAgent? SalesAgent { get; set; }

    /// <summary>
    /// Collection of orders for this service
    /// </summary>
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>
    /// Collection of quote lines for this service
    /// </summary>
    public ICollection<QuoteLine> QuoteLines { get; set; } = new List<QuoteLine>();
}

