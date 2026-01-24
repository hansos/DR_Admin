namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for Pesapal payment gateway
    /// </summary>
    public class PesapalSettings
    {
        /// <summary>
        /// Pesapal consumer key
        /// </summary>
        public string ConsumerKey { get; set; } = string.Empty;

        /// <summary>
        /// Pesapal consumer secret
        /// </summary>
        public string ConsumerSecret { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// IPN (Instant Payment Notification) URL
        /// </summary>
        public string IpnUrl { get; set; } = string.Empty;

        /// <summary>
        /// Callback URL after payment
        /// </summary>
        public string CallbackUrl { get; set; } = string.Empty;
    }
}
