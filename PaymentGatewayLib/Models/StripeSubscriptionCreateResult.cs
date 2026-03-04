namespace PaymentGatewayLib.Models
{
    public class StripeSubscriptionCreateResult
    {
        public bool Success { get; set; }
        public string SubscriptionId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
