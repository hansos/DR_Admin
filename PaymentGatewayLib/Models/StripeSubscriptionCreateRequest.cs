namespace PaymentGatewayLib.Models
{
    public class StripeSubscriptionCreateRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EUR";
        public string Interval { get; set; } = "month";
        public int IntervalCount { get; set; } = 1;
        public DateTime? TrialEndUtc { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
