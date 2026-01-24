namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for M-Pesa Daraja API payment gateway
    /// </summary>
    public class MpesaSettings
    {
        /// <summary>
        /// M-Pesa consumer key
        /// </summary>
        public string ConsumerKey { get; set; } = string.Empty;

        /// <summary>
        /// M-Pesa consumer secret
        /// </summary>
        public string ConsumerSecret { get; set; } = string.Empty;

        /// <summary>
        /// M-Pesa shortcode/paybill number
        /// </summary>
        public string Shortcode { get; set; } = string.Empty;

        /// <summary>
        /// M-Pesa passkey for STK Push
        /// </summary>
        public string Passkey { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; } = true;

        /// <summary>
        /// Callback URL for payment notifications
        /// </summary>
        public string CallbackUrl { get; set; } = string.Empty;
    }
}
