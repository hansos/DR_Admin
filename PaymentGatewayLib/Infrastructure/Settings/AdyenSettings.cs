namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Adyen payment gateway
    /// </summary>
    public class AdyenSettings
    {
        /// <summary>
        /// Adyen API key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Adyen merchant account
        /// </summary>
        public string MerchantAccount { get; set; } = string.Empty;

        /// <summary>
        /// Adyen environment (TEST or LIVE)
        /// </summary>
        public string Environment { get; set; } = "TEST";

        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL for live environment
        /// </summary>
        public string LiveApiUrl { get; set; } = "https://checkout-live.adyen.com";

        /// <summary>
        /// API base URL for test environment
        /// </summary>
        public string TestApiUrl { get; set; } = "https://checkout-test.adyen.com";

        /// <summary>
        /// HMAC key for webhook validation
        /// </summary>
        public string HmacKey { get; set; } = string.Empty;
    }
}
