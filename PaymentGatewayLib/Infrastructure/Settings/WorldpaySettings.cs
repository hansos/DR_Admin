namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Worldpay payment gateway
    /// </summary>
    public class WorldpaySettings
    {
        /// <summary>
        /// Worldpay service key
        /// </summary>
        public string ServiceKey { get; set; } = string.Empty;

        /// <summary>
        /// Worldpay client key (for client-side integration)
        /// </summary>
        public string ClientKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://api.worldpay.com";

        /// <summary>
        /// API base URL for test environment
        /// </summary>
        public string TestApiUrl { get; set; } = "https://api.worldpay.com/v1";

        /// <summary>
        /// Settlement currency
        /// </summary>
        public string SettlementCurrency { get; set; } = "USD";
    }
}
