namespace ISPAdmin.DTOs;

/// <summary>
/// Represents a payment transaction item for list views.
/// </summary>
public class PaymentTransactionListDto
{
    /// <summary>
    /// Gets or sets the payment transaction identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the related invoice identifier.
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Gets or sets the related invoice number.
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the related customer identifier.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the related customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the payment method used.
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the payment transaction status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the external gateway transaction identifier.
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the amount paid.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency code for the payment amount.
    /// </summary>
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets the gateway identifier.
    /// </summary>
    public int? PaymentGatewayId { get; set; }

    /// <summary>
    /// Gets or sets the gateway display name.
    /// </summary>
    public string PaymentGatewayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when payment processing completed.
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Gets or sets total refunded amount for this transaction.
    /// </summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>
    /// Gets or sets when the transaction was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets invoice payment allocations linked to this transaction.
    /// </summary>
    public List<PaymentTransactionAllocationDto> Allocations { get; set; } = [];
}

/// <summary>
/// Represents an invoice allocation row linked to a payment transaction.
/// </summary>
public class PaymentTransactionAllocationDto
{
    /// <summary>
    /// Gets or sets the invoice payment identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the amount applied to the invoice.
    /// </summary>
    public decimal AmountApplied { get; set; }

    /// <summary>
    /// Gets or sets the allocation currency code.
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Gets or sets the invoice balance after this allocation.
    /// </summary>
    public decimal InvoiceBalance { get; set; }

    /// <summary>
    /// Gets or sets the invoice total amount at allocation time.
    /// </summary>
    public decimal InvoiceTotalAmount { get; set; }

    /// <summary>
    /// Gets or sets whether this allocation completed full payment of invoice.
    /// </summary>
    public bool IsFullPayment { get; set; }

    /// <summary>
    /// Gets or sets when the allocation was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
