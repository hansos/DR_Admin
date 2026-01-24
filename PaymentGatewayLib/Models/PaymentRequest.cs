namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents a payment request to process a transaction
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// Amount to charge in the smallest currency unit (e.g., cents for USD)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Three-letter ISO currency code (e.g., USD, EUR, GBP)
        /// </summary>
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// Payment method token or identifier
        /// </summary>
        public string PaymentMethodToken { get; set; } = string.Empty;

        /// <summary>
        /// Customer email address
        /// </summary>
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Customer name
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Description of the transaction
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Unique reference identifier for the transaction
        /// </summary>
        public string ReferenceId { get; set; } = string.Empty;

        /// <summary>
        /// Additional metadata for the transaction
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Billing address information
        /// </summary>
        public BillingAddress? BillingAddress { get; set; }

        /// <summary>
        /// Whether to capture the payment immediately or just authorize
        /// </summary>
        public bool CaptureImmediately { get; set; } = true;
    }

    /// <summary>
    /// Represents billing address information
    /// </summary>
    public class BillingAddress
    {
        /// <summary>
        /// Street address line 1
        /// </summary>
        public string Line1 { get; set; } = string.Empty;

        /// <summary>
        /// Street address line 2
        /// </summary>
        public string Line2 { get; set; } = string.Empty;

        /// <summary>
        /// City name
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// State or province
        /// </summary>
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// Postal or ZIP code
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Two-letter ISO country code
        /// </summary>
        public string Country { get; set; } = string.Empty;
    }
}
