namespace ISPAdmin.PaymentGateways;

/// <summary>
/// Request for charging a customer
/// </summary>
public class ChargeRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string PaymentMethodToken { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
    public bool Capture { get; set; } = true;
}

/// <summary>
/// Request for creating a payment intent
/// </summary>
public class PaymentIntentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string PaymentMethodToken { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
    public string ReturnUrl { get; set; } = string.Empty;
}

/// <summary>
/// Request for saving a payment method
/// </summary>
public class SavePaymentMethodRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string PaymentMethodToken { get; set; } = string.Empty;
    public bool SetAsDefault { get; set; }
}

/// <summary>
/// Request for processing a refund
/// </summary>
public class RefundRequest
{
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Result from gateway charge operation
/// </summary>
public class GatewayChargeResult
{
    public bool IsSuccess { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Result from gateway payment intent operation
/// </summary>
public class GatewayPaymentIntentResult
{
    public bool IsSuccess { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool RequiresAction { get; set; }
    public string AuthenticationUrl { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Result from saving payment method
/// </summary>
public class GatewayPaymentMethodResult
{
    public bool IsSuccess { get; set; }
    public string PaymentMethodId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string Last4Digits { get; set; } = string.Empty;
    public string CardBrand { get; set; } = string.Empty;
    public int? ExpiryMonth { get; set; }
    public int? ExpiryYear { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Result from refund operation
/// </summary>
public class GatewayRefundResult
{
    public bool IsSuccess { get; set; }
    public string RefundId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Webhook event from payment gateway
/// </summary>
public class GatewayWebhookEvent
{
    public string EventType { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}
