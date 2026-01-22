namespace ISPAdmin.Data.Entities;

public class InvoiceLine
{
    // Primary Key
    public int Id { get; set; }

    // Relationships
    public int InvoiceId { get; set; }
    public int? ServiceId { get; set; }
    public int? UnitId { get; set; }

    // Line details
    public int LineNumber { get; set; } = 1;            // Order on invoice
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; } = 0;          // Absolute value
    public decimal TotalPrice { get; set; }             // Quantity * UnitPrice - Discount
    public decimal TaxRate { get; set; } = 0;           // e.g., 0.25 for 25% VAT
    public decimal TaxAmount { get; set; }              // Total tax for this line
    public decimal TotalWithTax { get; set; }           // TotalPrice + TaxAmount

    // Snapshot for historical integrity
    public string ServiceNameSnapshot { get; set; } = string.Empty; 
    public string AccountingCode { get; set; } = string.Empty; // GL / SKU / product code

    // Audit & lifecycle
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }            // Soft delete support

    // Optional metadata
    public string Notes { get; set; } = string.Empty;

    // Navigation Properties
    public Invoice Invoice { get; set; } = null!;
    public Service? Service { get; set; }
    public Unit Unit { get; set; } = null!;
}
