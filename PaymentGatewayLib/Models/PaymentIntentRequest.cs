namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents a request to create a payment intent
    /// </summary>
    public class PaymentIntentRequest
    {
        /// <summary>
        /// Amount to charge in the smallest currency unit
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Three-letter ISO currency code
        /// </summary>
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// Customer email address
        /// </summary>
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Customer identifier if already exists in the gateway
        /// </summary>
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        /// Description of the payment
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Additional metadata for the payment intent
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Whether to capture the payment automatically
        /// </summary>
        public bool AutomaticCapture { get; set; } = true;
    }
}
