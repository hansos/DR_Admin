namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents the result of creating a customer profile
    /// </summary>
    public class CustomerProfileResult
    {
        /// <summary>
        /// Indicates whether the profile was created successfully
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Customer identifier in the payment gateway
        /// </summary>
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        /// Customer email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Customer name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Error message if creation failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Error code if creation failed
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the profile was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
