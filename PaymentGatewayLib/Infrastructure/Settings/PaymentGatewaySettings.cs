namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Main configuration settings for payment gateway providers
    /// </summary>
    public class PaymentGatewaySettings
    {
        /// <summary>
        /// Payment provider to use (stripe, paypal, square, adyen, braintree, etc.)
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

        /// <summary>
        /// Adyen payment gateway settings
        /// </summary>
        public AdyenSettings? Adyen { get; set; }

        /// <summary>
        /// Braintree payment gateway settings
        /// </summary>
        public BraintreeSettings? Braintree { get; set; }

        /// <summary>
        /// Worldpay payment gateway settings
        /// </summary>
        public WorldpaySettings? Worldpay { get; set; }

        /// <summary>
        /// Authorize.Net payment gateway settings
        /// </summary>
        public AuthorizeNetSettings? AuthorizeNet { get; set; }

        /// <summary>
        /// Klarna payment gateway settings
        /// </summary>
        public KlarnaSettings? Klarna { get; set; }

        /// <summary>
        /// Mollie payment gateway settings
        /// </summary>
        public MollieSettings? Mollie { get; set; }

        /// <summary>
        /// Checkout.com payment gateway settings
        /// </summary>
        public CheckoutComSettings? CheckoutCom { get; set; }

        /// <summary>
        /// PayU payment gateway settings
        /// </summary>
        public PayUSettings? PayU { get; set; }

        /// <summary>
        /// GoCardless payment gateway settings
        /// </summary>
        public GoCardlessSettings? GoCardless { get; set; }

        /// <summary>
        /// Trustly payment gateway settings
        /// </summary>
        public TrustlySettings? Trustly { get; set; }

        /// <summary>
        /// Vipps payment gateway settings
        /// </summary>
        public VippsSettings? Vipps { get; set; }

        /// <summary>
        /// Nets payment gateway settings
        /// </summary>
        public NetsSettings? Nets { get; set; }

        /// <summary>
        /// Elavon payment gateway settings
        /// </summary>
        public ElavonSettings? Elavon { get; set; }

        /// <summary>
        /// Cybersource payment gateway settings
        /// </summary>
        public CybersourceSettings? Cybersource { get; set; }

        /// <summary>
        /// BitPay payment gateway settings (cryptocurrency)
        /// </summary>
        public BitPaySettings? BitPay { get; set; }

        /// <summary>
        /// OpenNode payment gateway settings (cryptocurrency - Bitcoin Lightning)
        /// </summary>
        public OpenNodeSettings? OpenNode { get; set; }

        /// <summary>
        /// M-Pesa Daraja API payment gateway settings (African mobile money)
        /// </summary>
        public MpesaSettings? Mpesa { get; set; }

        /// <summary>
        /// Flutterwave payment gateway settings (African payments)
        /// </summary>
        public FlutterwaveSettings? Flutterwave { get; set; }

        /// <summary>
        /// Pesapal payment gateway settings (African payments)
        /// </summary>
        public PesapalSettings? Pesapal { get; set; }

        /// <summary>
        /// Paystack payment gateway settings (African payments)
        /// </summary>
        public PaystackSettings? Paystack { get; set; }

        /// <summary>
        /// iPay Africa payment gateway settings
        /// </summary>
        public IPayAfricaSettings? IPayAfrica { get; set; }

        /// <summary>
        /// DPO Group (DPO Pay) payment gateway settings (African payments)
        /// </summary>
        public DpoGroupSettings? DpoGroup { get; set; }

        /// <summary>
        /// IntaSend payment gateway settings (African payments)
        /// </summary>
        public IntaSendSettings? IntaSend { get; set; }

        /// <summary>
        /// JamboPay payment gateway settings (African payments)
        /// </summary>
        public JamboPaySettings? JamboPay { get; set; }

        /// <summary>
        /// KopoKopo payment gateway settings (African payments)
        /// </summary>
        public KopoKopoSettings? KopoKopo { get; set; }

        /// <summary>
        /// Africa's Talking payment gateway settings (African mobile money & payments)
        /// </summary>
        public AfricasTalkingSettings? AfricasTalking { get; set; }
    }
}
