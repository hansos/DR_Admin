namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents the result of capturing an authorized payment
    /// </summary>
    public class CaptureResult
    {
        /// <summary>
        /// Indicates whether the capture was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Transaction identifier for the capture
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Original authorization identifier
        /// </summary>
        public string AuthorizationId { get; set; } = string.Empty;

        /// <summary>
        /// Amount that was captured
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code of the capture
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the capture
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Error message if the capture failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Error code if the capture failed
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the capture was processed
        /// </summary>
        public DateTime ProcessedAt { get; set; }
    }
}
