namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents the result of a refund transaction
    /// </summary>
    public class RefundResult
    {
        /// <summary>
        /// Indicates whether the refund was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Refund transaction identifier
        /// </summary>
        public string RefundId { get; set; } = string.Empty;

        /// <summary>
        /// Original transaction identifier that was refunded
        /// </summary>
        public string OriginalTransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Amount that was refunded
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code of the refund
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the refund
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Error message if the refund failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Error code if the refund failed
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the refund was processed
        /// </summary>
        public DateTime ProcessedAt { get; set; }

        /// <summary>
        /// Raw response from the payment gateway for debugging
        /// </summary>
        public string RawResponse { get; set; } = string.Empty;
    }
}
