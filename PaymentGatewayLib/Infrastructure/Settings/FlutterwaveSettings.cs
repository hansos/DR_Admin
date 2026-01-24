namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Flutterwave payment gateway
    /// </summary>
    public class FlutterwaveSettings
    {
        /// <summary>
        /// Flutterwave public key
        /// </summary>
        public string PublicKey { get; set; } = string.Empty;

        /// <summary>
        /// Flutterwave secret key
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Flutterwave encryption key
        /// </summary>
        public string EncryptionKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// Redirect URL after payment
        /// </summary>
        public string RedirectUrl { get; set; } = string.Empty;
    }
}
