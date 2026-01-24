namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents the status of a payment transaction
    /// </summary>
    public class TransactionStatusResult
    {
        /// <summary>
        /// Transaction identifier
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the transaction
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Amount of the transaction
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code of the transaction
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the transaction was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the transaction was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Customer email associated with the transaction
        /// </summary>
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Description of the transaction
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Additional metadata about the transaction
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
