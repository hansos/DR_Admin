namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Africa's Talking payment gateway
    /// </summary>
    public class AfricasTalkingSettings
    {
        /// <summary>
        /// Africa's Talking username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Africa's Talking API key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Product name for payments
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// Provider channel (e.g., Athena for mobile money)
        /// </summary>
        public string ProviderChannel { get; set; } = string.Empty;
    }
}
