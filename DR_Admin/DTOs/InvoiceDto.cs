using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

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
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public string CustomerTaxId { get; set; } = string.Empty;
    public string PaymentReference { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string InternalComment { get; set; } = string.Empty;
}

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

