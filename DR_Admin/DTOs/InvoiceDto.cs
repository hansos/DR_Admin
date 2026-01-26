using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing an invoice
/// </summary>
public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }
    public string CurrencyCode { get; set; } = "EUR";
    public decimal TaxRate { get; set; }
    public string TaxName { get; set; } = "VAT";
    
    /// <summary>
    /// Base currency for accounting purposes
    /// </summary>
    public string BaseCurrencyCode { get; set; } = "EUR";
    
    /// <summary>
    /// Display currency for customer
    /// </summary>
    public string DisplayCurrencyCode { get; set; } = "EUR";
    
    /// <summary>
    /// Exchange rate applied at invoice creation
    /// </summary>
    public decimal? ExchangeRate { get; set; }
    
    /// <summary>
    /// Total amount in base currency for accounting
    /// </summary>
    public decimal? BaseTotalAmount { get; set; }
    
    /// <summary>
    /// Date of the exchange rate used
    /// </summary>
    public DateTime? ExchangeRateDate { get; set; }
    
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public string CustomerTaxId { get; set; } = string.Empty;
    public string PaymentReference { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string InternalComment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new invoice
/// </summary>
public class CreateInvoiceDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = "EUR";
    public decimal TaxRate { get; set; }
    public string TaxName { get; set; } = "VAT";
    
    /// <summary>
    /// Display currency for customer (optional, will use CurrencyCode if not specified)
    /// </summary>
    public string? DisplayCurrencyCode { get; set; }
    
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public string CustomerTaxId { get; set; } = string.Empty;
    public string PaymentReference { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string InternalComment { get; set; } = string.Empty;
}


/// <summary>
/// Data transfer object for updating an existing invoice
/// </summary>
public class UpdateInvoiceDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }
    public string CurrencyCode { get; set; } = "EUR";
    public decimal TaxRate { get; set; }
    public string TaxName { get; set; } = "VAT";
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public string CustomerTaxId { get; set; } = string.Empty;
    public string PaymentReference { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string InternalComment { get; set; } = string.Empty;
}

