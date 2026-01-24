namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Klarna payment gateway
    /// </summary>
    public class KlarnaSettings
    {
        /// <summary>
        /// Klarna username (merchant ID)
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Klarna password (shared secret)
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test environment
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// API base URL for production (EU)
        /// </summary>
        public string ProductionApiUrlEU { get; set; } = "https://api.klarna.com";

        /// <summary>
        /// API base URL for production (NA)
        /// </summary>
        public string ProductionApiUrlNA { get; set; } = "https://api-na.klarna.com";

        /// <summary>
        /// API base URL for playground/test
        /// </summary>
        public string PlaygroundApiUrl { get; set; } = "https://api.playground.klarna.com";

        /// <summary>
        /// Region (EU or NA)
        /// </summary>
        public string Region { get; set; } = "EU";
    }
}
