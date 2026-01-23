namespace ISPAdmin.Data.Entities;

public class Unit : EntityBase
{
    // Unit details
    public string Code { get; set; } = string.Empty;        // e.g., "pcs", "hrs", "GB", "month"
    public string Name { get; set; } = string.Empty;        // e.g., "Pieces", "Hours", "Gigabytes", "Month"
    public string Description { get; set; } = string.Empty; // Optional description
    public bool IsActive { get; set; } = true;              // Active/inactive flag

    // Lifecycle
    public DateTime? DeletedAt { get; set; }                // Soft delete support

    // Navigation Properties
    public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
}
