namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for JamboPay payment gateway
    /// </summary>
    public class JamboPaySettings
    {
        /// <summary>
        /// JamboPay merchant ID
        /// </summary>
        public string MerchantId { get; set; } = string.Empty;

        /// <summary>
        /// JamboPay API key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// Callback URL for payment notifications
        /// </summary>
        public string CallbackUrl { get; set; } = string.Empty;

        /// <summary>
        /// API endpoint URL
        /// </summary>
        public string ApiEndpoint { get; set; } = string.Empty;
    }
}
