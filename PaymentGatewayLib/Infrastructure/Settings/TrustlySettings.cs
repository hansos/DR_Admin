namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Trustly payment gateway
    /// </summary>
    public class TrustlySettings
    {
        /// <summary>
        /// Trustly username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Trustly password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Private key for signing requests
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test environment
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://api.trustly.com";

        /// <summary>
        /// API base URL for test
        /// </summary>
        public string TestApiUrl { get; set; } = "https://test.trustly.com";
    }
}
