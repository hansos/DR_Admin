namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Authorize.Net payment gateway
    /// </summary>
    public class AuthorizeNetSettings
    {
        /// <summary>
        /// Authorize.Net API login ID
        /// </summary>
        public string ApiLoginId { get; set; } = string.Empty;

        /// <summary>
        /// Authorize.Net transaction key
        /// </summary>
        public string TransactionKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://api.authorize.net";

        /// <summary>
        /// API base URL for sandbox
        /// </summary>
        public string SandboxApiUrl { get; set; } = "https://apitest.authorize.net";

        /// <summary>
        /// Signature key for webhook validation
        /// </summary>
        public string SignatureKey { get; set; } = string.Empty;
    }
}
