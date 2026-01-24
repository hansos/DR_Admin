namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Nets payment gateway
    /// </summary>
    public class NetsSettings
    {
        /// <summary>
        /// Nets secret key
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Nets checkout key
        /// </summary>
        public string CheckoutKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test environment
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://api.dibspayment.eu";

        /// <summary>
        /// API base URL for test
        /// </summary>
        public string TestApiUrl { get; set; } = "https://test.api.dibspayment.eu";

        /// <summary>
        /// Merchant ID
        /// </summary>
        public string MerchantId { get; set; } = string.Empty;
    }
}
