namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for PayPal payment gateway
    /// </summary>
    public class PayPalSettings
    {
        /// <summary>
        /// PayPal client ID
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// PayPal client secret
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// PayPal environment (sandbox or live)
        /// </summary>
        public string Environment { get; set; } = "sandbox";

        /// <summary>
        /// Whether to use sandbox mode
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// API base URL for live environment
        /// </summary>
        public string LiveApiUrl { get; set; } = "https://api-m.paypal.com";

        /// <summary>
        /// API base URL for sandbox environment
        /// </summary>
        public string SandboxApiUrl { get; set; } = "https://api-m.sandbox.paypal.com";

        /// <summary>
        /// Webhook ID for validating webhooks
        /// </summary>
        public string WebhookId { get; set; } = string.Empty;
    }
}
