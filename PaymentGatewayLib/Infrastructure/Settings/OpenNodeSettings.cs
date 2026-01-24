namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for OpenNode payment gateway (cryptocurrency - Bitcoin Lightning Network)
    /// </summary>
    public class OpenNodeSettings
    {
        /// <summary>
        /// OpenNode API key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test environment
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://api.opennode.com";

        /// <summary>
        /// API base URL for test/dev
        /// </summary>
        public string TestApiUrl { get; set; } = "https://dev-api.opennode.com";

        /// <summary>
        /// Callback URL for payment notifications
        /// </summary>
        public string CallbackUrl { get; set; } = string.Empty;
    }
}
