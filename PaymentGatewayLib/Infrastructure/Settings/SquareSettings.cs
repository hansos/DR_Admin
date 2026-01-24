namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Square payment gateway
    /// </summary>
    public class SquareSettings
    {
        /// <summary>
        /// Square access token
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Square application ID
        /// </summary>
        public string ApplicationId { get; set; } = string.Empty;

        /// <summary>
        /// Square location ID
        /// </summary>
        public string LocationId { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use sandbox mode
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// API base URL for sandbox environment
        /// </summary>
        public string SandboxApiUrl { get; set; } = "https://connect.squareupsandbox.com";

        /// <summary>
        /// API base URL for production environment
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://connect.squareup.com";

        /// <summary>
        /// Webhook signature key for validating webhooks
        /// </summary>
        public string WebhookSignatureKey { get; set; } = string.Empty;
    }
}
