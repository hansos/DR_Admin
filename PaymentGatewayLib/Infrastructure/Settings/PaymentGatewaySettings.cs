namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Main configuration settings for payment gateway providers
    /// </summary>
    public class PaymentGatewaySettings
    {
        /// <summary>
        /// Payment provider to use (stripe, paypal, square, etc.)
        /// </summary>
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Stripe payment gateway settings
        /// </summary>
        public StripeSettings? Stripe { get; set; }

        /// <summary>
        /// PayPal payment gateway settings
        /// </summary>
        public PayPalSettings? PayPal { get; set; }

        /// <summary>
        /// Square payment gateway settings
        /// </summary>
        public SquareSettings? Square { get; set; }
    }
}
