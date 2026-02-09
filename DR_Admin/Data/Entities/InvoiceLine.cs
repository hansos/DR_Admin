using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

public class InvoiceLine : EntityBase
{
    // Relationships
    public int InvoiceId { get; set; }
    public int? ServiceId { get; set; }
    public int? UnitId { get; set; }

    // Line details
    public int LineNumber { get; set; } = 1;            // Order on invoice
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    
    /// <summary>
    /// Type of line item (product, service, fee, etc.)
    /// </summary>
    public InvoiceLineType LineType { get; set; } = InvoiceLineType.Product;
    
    /// <summary>
    /// Indicates if this line represents a payment gateway fee
    /// </summary>
    public bool IsGatewayFee { get; set; } = false;
    
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; } = 0;          // Absolute value
    public decimal TotalPrice { get; set; }             // Quantity * UnitPrice - Discount
    public decimal TaxRate { get; set; } = 0;           // e.g., 0.25 for 25% VAT
    public decimal TaxAmount { get; set; }              // Total tax for this line
    public decimal TotalWithTax { get; set; }           // TotalPrice + TaxAmount

    // Snapshot for historical integrity
    public string ServiceNameSnapshot { get; set; } = string.Empty; 
    public string AccountingCode { get; set; } = string.Empty; // GL / SKU / product code

    // Lifecycle
    public DateTime? DeletedAt { get; set; }            // Soft delete support

    // Optional metadata
    public string Notes { get; set; } = string.Empty;

    // Navigation Properties
    public Invoice Invoice { get; set; } = null!;
    public Service? Service { get; set; }
    public Unit Unit { get; set; } = null!;
    
    /// <summary>
    /// Collection of vendor costs associated with this line item
    /// </summary>
    public ICollection<VendorCost> VendorCosts { get; set; } = new List<VendorCost>();
}
