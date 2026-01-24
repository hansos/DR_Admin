namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for GoCardless payment gateway
    /// </summary>
    public class GoCardlessSettings
    {
        /// <summary>
        /// GoCardless access token
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://api.gocardless.com";

        /// <summary>
        /// API base URL for sandbox
        /// </summary>
        public string SandboxApiUrl { get; set; } = "https://api-sandbox.gocardless.com";

        /// <summary>
        /// Webhook secret for validation
        /// </summary>
        public string WebhookSecret { get; set; } = string.Empty;

        /// <summary>
        /// GoCardless version
        /// </summary>
        public string Version { get; set; } = "2015-07-06";
    }
}
