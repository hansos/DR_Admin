namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for KopoKopo payment gateway
    /// </summary>
    public class KopoKopoSettings
    {
        /// <summary>
        /// KopoKopo client ID
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// KopoKopo client secret
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// KopoKopo API key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// Webhook secret for verifying callbacks
        /// </summary>
        public string WebhookSecret { get; set; } = string.Empty;
    }
}
