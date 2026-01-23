namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a line item on an invoice
/// </summary>
public class InvoiceLineDto
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int? ServiceId { get; set; }
    public int UnitId { get; set; }
    
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalWithTax { get; set; }
    
    public string ServiceNameSnapshot { get; set; } = string.Empty;
    public string AccountingCode { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public string Notes { get; set; } = string.Empty;
}


/// <summary>
/// Data transfer object for creating a new invoice line item
/// </summary>
public class CreateInvoiceLineDto
{
    public int InvoiceId { get; set; }
    public int? ServiceId { get; set; }
    
    public int LineNumber { get; set; } = 1;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = "pcs";
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; } = 0;
    public decimal TotalPrice { get; set; }
    public decimal TaxRate { get; set; } = 0;
    public decimal TaxAmount { get; set; }
    public decimal TotalWithTax { get; set; }
    
    public string ServiceNameSnapshot { get; set; } = string.Empty;
    public string AccountingCode { get; set; } = string.Empty;
    
    public string Notes { get; set; } = string.Empty;
}

public class UpdateInvoiceLineDto
{
    public int InvoiceId { get; set; }
    public int? ServiceId { get; set; }
    
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalWithTax { get; set; }
    
    public string ServiceNameSnapshot { get; set; } = string.Empty;
    public string AccountingCode { get; set; } = string.Empty;
    
    public string Notes { get; set; } = string.Empty;
}
