namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Cybersource payment gateway
    /// </summary>
    public class CybersourceSettings
    {
        /// <summary>
        /// Cybersource merchant ID
        /// </summary>
        public string MerchantId { get; set; } = string.Empty;

        /// <summary>
        /// Cybersource API key ID
        /// </summary>
        public string ApiKeyId { get; set; } = string.Empty;

        /// <summary>
        /// Cybersource shared secret key
        /// </summary>
        public string SharedSecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test environment
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://api.cybersource.com";

        /// <summary>
        /// API base URL for test
        /// </summary>
        public string TestApiUrl { get; set; } = "https://apitest.cybersource.com";
    }
}
