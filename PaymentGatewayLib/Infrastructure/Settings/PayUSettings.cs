namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for PayU payment gateway
    /// </summary>
    public class PayUSettings
    {
        /// <summary>
        /// PayU POS ID (point of sale identifier)
        /// </summary>
        public string PosId { get; set; } = string.Empty;

        /// <summary>
        /// PayU client ID
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// PayU client secret
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// API base URL for production
        /// </summary>
        public string ProductionApiUrl { get; set; } = "https://secure.payu.com";

        /// <summary>
        /// API base URL for sandbox
        /// </summary>
        public string SandboxApiUrl { get; set; } = "https://secure.snd.payu.com";

        /// <summary>
        /// Second key for signature validation
        /// </summary>
        public string SecondKey { get; set; } = string.Empty;
    }
}
