namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Represents a refund request for a previous payment
    /// </summary>
    public class RefundRequest
    {
        /// <summary>
        /// Original transaction identifier to refund
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Amount to refund (null for full refund)
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Reason for the refund
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Additional metadata for the refund
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
