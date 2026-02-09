using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a payment attempt
/// </summary>
public class PaymentAttemptDto
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int? PaymentTransactionId { get; set; }
    public int CustomerPaymentMethodId { get; set; }
    public decimal AttemptedAmount { get; set; }
    public string Currency { get; set; } = "EUR";
    public PaymentAttemptStatus Status { get; set; }
    public string GatewayResponse { get; set; } = string.Empty;
    public string GatewayTransactionId { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public DateTime? NextRetryAt { get; set; }
    public bool RequiresAuthentication { get; set; }
    public string AuthenticationUrl { get; set; } = string.Empty;
    public AuthenticationStatus AuthenticationStatus { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request to process an invoice payment
/// </summary>
public class ProcessInvoicePaymentDto
{
    public int InvoiceId { get; set; }
    public int CustomerPaymentMethodId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
}

/// <summary>
/// Result from payment processing
/// </summary>
public class PaymentResultDto
{
    public bool IsSuccess { get; set; }
    public bool RequiresAuthentication { get; set; }
    public string AuthenticationUrl { get; set; } = string.Empty;
    public int? PaymentAttemptId { get; set; }
    public int? PaymentTransactionId { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}

/// <summary>
/// Request to apply customer credit to invoice
/// </summary>
public class ApplyCustomerCreditDto
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Request for partial payment
/// </summary>
public class ProcessPartialPaymentDto
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public int CustomerPaymentMethodId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

/// <summary>
/// Payment method token DTO
/// </summary>
public class PaymentMethodTokenDto
{
    public int Id { get; set; }
    public int CustomerPaymentMethodId { get; set; }
    public string GatewayCustomerId { get; set; } = string.Empty;
    public string GatewayPaymentMethodId { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public string Last4Digits { get; set; } = string.Empty;
    public string CardBrand { get; set; } = string.Empty;
    public int? ExpiryMonth { get; set; }
    public int? ExpiryYear { get; set; }
    public bool IsDefault { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Invoice payment DTO
/// </summary>
public class InvoicePaymentDto
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int PaymentTransactionId { get; set; }
    public decimal AmountApplied { get; set; }
    public string Currency { get; set; } = "EUR";
    public decimal InvoiceBalance { get; set; }
    public decimal InvoiceTotalAmount { get; set; }
    public bool IsFullPayment { get; set; }
    public DateTime CreatedAt { get; set; }
}
