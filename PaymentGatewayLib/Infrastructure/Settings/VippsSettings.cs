namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Vipps payment gateway
    /// </summary>
    public class VippsSettings
    {
        /// <summary>
        /// Vipps client ID
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Vipps client secret
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// Vipps subscription key (Ocp-Apim-Subscription-Key)
        /// </summary>
        public string SubscriptionKey { get; set; } = string.Empty;

        /// <summary>
        /// Merchant serial number
        /// </summary>
        public string MerchantSerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test environment
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://api.vipps.no";

        /// <summary>
        /// API base URL for test
        /// </summary>
        public string TestApiUrl { get; set; } = "https://apitest.vipps.no";
    }
}
