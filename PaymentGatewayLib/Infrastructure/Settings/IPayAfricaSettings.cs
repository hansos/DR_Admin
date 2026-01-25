namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for iPay Africa payment gateway
    /// </summary>
    public class IPayAfricaSettings
    {
        /// <summary>
        /// iPay Africa vendor ID
        /// </summary>
        public string VendorId { get; set; } = string.Empty;

        /// <summary>
        /// iPay Africa hash key
        /// </summary>
        public string HashKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// Auto redirect after payment (0 or 1)
        /// </summary>
        public int AutoRedirect { get; set; } = 1;

        /// <summary>
        /// Callback URL for payment notifications
        /// </summary>
        public string CallbackUrl { get; set; } = string.Empty;

        /// <summary>
        /// iPay Africa API base URL for transaction status and other API calls
        /// Default: https://apis.ipayafrica.com
        /// </summary>
        public string ApiBaseUrl { get; set; } = "https://apis.ipayafrica.com";

        /// <summary>
        /// Transaction status endpoint path
        /// Default: /payments/v2/transact/mobilemoney/status
        /// </summary>
        public string StatusEndpoint { get; set; } = "/payments/v2/transact/mobilemoney/status";

        /// <summary>
        /// API authentication token (if required)
        /// </summary>
        public string ApiToken { get; set; } = string.Empty;

        /// <summary>
        /// Request timeout in seconds
        /// Default: 30 seconds
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}

