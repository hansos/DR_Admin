namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for BitPay payment gateway (cryptocurrency)
    /// </summary>
    public class BitPaySettings
    {
        /// <summary>
        /// BitPay API token
        /// </summary>
        public string ApiToken { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test environment
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://bitpay.com";

        /// <summary>
        /// API base URL for test
        /// </summary>
        public string TestApiUrl { get; set; } = "https://test.bitpay.com";

        /// <summary>
        /// Private key for signing requests
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// Notification URL for IPN callbacks
        /// </summary>
        public string NotificationUrl { get; set; } = string.Empty;
    }
}
