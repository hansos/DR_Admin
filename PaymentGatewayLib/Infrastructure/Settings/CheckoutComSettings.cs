namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Checkout.com payment gateway
    /// </summary>
    public class CheckoutComSettings
    {
        /// <summary>
        /// Checkout.com secret key
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Checkout.com public key
        /// </summary>
        public string PublicKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://api.checkout.com";

        /// <summary>
        /// API base URL for sandbox
        /// </summary>
        public string SandboxApiUrl { get; set; } = "https://api.sandbox.checkout.com";

        /// <summary>
        /// Processing channel ID
        /// </summary>
        public string ProcessingChannelId { get; set; } = string.Empty;
    }
}
