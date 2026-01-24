namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents the result of creating a payment intent
    /// </summary>
    public class PaymentIntentResult
    {
        /// <summary>
        /// Indicates whether the intent was created successfully
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Payment intent identifier
        /// </summary>
        public string IntentId { get; set; } = string.Empty;

        /// <summary>
        /// Client secret for completing the payment on client side
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the payment intent
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Amount of the payment intent
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code of the payment intent
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Error message if creation failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Error code if creation failed
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the intent was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
