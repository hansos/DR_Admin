namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Stripe payment gateway
    /// </summary>
    public class StripeSettings
    {
        /// <summary>
        /// Stripe API secret key
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Stripe publishable key for client-side integration
        /// </summary>
        public string PublishableKey { get; set; } = string.Empty;

        /// <summary>
        /// Webhook signing secret for validating webhooks
        /// </summary>
        public string WebhookSecret { get; set; } = string.Empty;

        /// <summary>
        /// API version to use (e.g., 2023-10-16)
        /// </summary>
        public string ApiVersion { get; set; } = "2023-10-16";

        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool TestMode { get; set; } = false;

        /// <summary>
        /// API base URL (typically https://api.stripe.com)
        /// </summary>
        public string ApiBaseUrl { get; set; } = "https://api.stripe.com";
    }
}
