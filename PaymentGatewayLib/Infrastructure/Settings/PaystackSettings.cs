namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Paystack payment gateway
    /// </summary>
    public class PaystackSettings
    {
        /// <summary>
        /// Paystack public key
        /// </summary>
        public string PublicKey { get; set; } = string.Empty;

        /// <summary>
        /// Paystack secret key
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// Callback URL for payment notifications
        /// </summary>
        public string CallbackUrl { get; set; } = string.Empty;

        /// <summary>
        /// Merchant email address
        /// </summary>
        public string MerchantEmail { get; set; } = string.Empty;
    }
}
