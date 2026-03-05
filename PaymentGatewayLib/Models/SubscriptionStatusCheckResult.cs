namespace PaymentGatewayLib.Models
{
    /// <summary>
    /// Result for checking subscription status in a payment gateway.
    /// </summary>
    public class SubscriptionStatusCheckResult
    {
        /// <summary>
        /// Indicates whether the check call succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Indicates whether this gateway supports subscription status checks.
        /// </summary>
        public bool IsSupported { get; set; }

        /// <summary>
        /// Gateway external subscription identifier.
        /// </summary>
        public string ExternalSubscriptionId { get; set; } = string.Empty;

        /// <summary>
        /// Raw gateway status value.
        /// </summary>
        public string GatewayStatus { get; set; } = string.Empty;

        /// <summary>
        /// Normalized subscription status.
        /// </summary>
        public SubscriptionStatusCheckState Status { get; set; } = SubscriptionStatusCheckState.Unknown;

        /// <summary>
        /// Error message if the check failed.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// UTC timestamp for this check.
        /// </summary>
        public DateTime CheckedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
