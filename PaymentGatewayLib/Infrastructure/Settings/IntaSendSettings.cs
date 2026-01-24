namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for IntaSend payment gateway
    /// </summary>
    public class IntaSendSettings
    {
        /// <summary>
        /// IntaSend publishable key
        /// </summary>
        public string PublishableKey { get; set; } = string.Empty;

        /// <summary>
        /// IntaSend secret key
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL
        /// </summary>
        public string ApiBaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Webhook secret for verifying callbacks
        /// </summary>
        public string WebhookSecret { get; set; } = string.Empty;
    }
}
