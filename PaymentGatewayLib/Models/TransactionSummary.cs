namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents a summary of a payment transaction
    /// </summary>
    public class TransactionSummary
    {
        /// <summary>
        /// Transaction identifier
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Type of transaction (payment, refund, capture, etc.)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the transaction
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Transaction amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Customer email
        /// </summary>
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Transaction description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the transaction was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the transaction was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
