namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Mollie payment gateway
    /// </summary>
    public class MollieSettings
    {
        /// <summary>
        /// Mollie API key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL
        /// </summary>
        public string ApiBaseUrl { get; set; } = "https://api.mollie.com";

        /// <summary>
        /// Profile ID (optional)
        /// </summary>
        public string ProfileId { get; set; } = string.Empty;

        /// <summary>
        /// Webhook URL for payment notifications
        /// </summary>
        public string WebhookUrl { get; set; } = string.Empty;
    }
}
