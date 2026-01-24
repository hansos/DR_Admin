namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents the result of a payment transaction
    /// </summary>
    public class PaymentResult
    {
        /// <summary>
        /// Indicates whether the payment was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Transaction identifier from the payment gateway
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Authorization code if payment was authorized
        /// </summary>
        public string AuthorizationCode { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the transaction
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Amount that was charged
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code of the transaction
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Error message if the transaction failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Error code if the transaction failed
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the transaction was processed
        /// </summary>
        public DateTime ProcessedAt { get; set; }

        /// <summary>
        /// Additional metadata returned by the gateway
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Raw response from the payment gateway for debugging
        /// </summary>
        public string RawResponse { get; set; } = string.Empty;
    }
}
