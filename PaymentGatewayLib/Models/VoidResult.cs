namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents the result of voiding an authorized payment
    /// </summary>
    public class VoidResult
    {
        /// <summary>
        /// Indicates whether the void was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Authorization identifier that was voided
        /// </summary>
        public string AuthorizationId { get; set; } = string.Empty;

        /// <summary>
        /// Current status after void
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Error message if the void failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Error code if the void failed
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the void was processed
        /// </summary>
        public DateTime ProcessedAt { get; set; }
    }
}
