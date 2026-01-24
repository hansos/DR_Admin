namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Braintree payment gateway
    /// </summary>
    public class BraintreeSettings
    {
        /// <summary>
        /// Braintree merchant ID
        /// </summary>
        public string MerchantId { get; set; } = string.Empty;

        /// <summary>
        /// Braintree public key
        /// </summary>
        public string PublicKey { get; set; } = string.Empty;

        /// <summary>
        /// Braintree private key
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// Braintree merchant account ID (optional)
        /// </summary>
        public string MerchantAccountId { get; set; } = string.Empty;
    }
}
