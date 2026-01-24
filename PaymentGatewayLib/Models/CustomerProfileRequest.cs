namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents a request to create a customer profile
    /// </summary>
    public class CustomerProfileRequest
    {
        /// <summary>
        /// Customer email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Customer full name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Customer phone number
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Customer description or notes
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Billing address for the customer
        /// </summary>
        public BillingAddress? Address { get; set; }

        /// <summary>
        /// Additional metadata for the customer profile
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
