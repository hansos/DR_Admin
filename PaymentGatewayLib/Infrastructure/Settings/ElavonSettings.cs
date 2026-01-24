namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Elavon payment gateway
    /// </summary>
    public class ElavonSettings
    {
        /// <summary>
        /// Elavon merchant ID
        /// </summary>
        public string MerchantId { get; set; } = string.Empty;

        /// <summary>
        /// Elavon user ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Elavon PIN
        /// </summary>
        public string Pin { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test environment
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://www.myvirtualmerchant.com";

        /// <summary>
        /// API base URL for test
        /// </summary>
        public string TestApiUrl { get; set; } = "https://demo.myvirtualmerchant.com";
    }
}
