using PaymentGatewayLib.Implementations;
using PaymentGatewayLib.Infrastructure.Settings;
using PaymentGatewayLib.Interfaces;

namespace PaymentGatewayLib.Factories
{
    /// <summary>
    /// Factory for creating payment gateway instances based on configuration
    /// </summary>
    public class PaymentGatewayFactory
    {
        private readonly PaymentGatewaySettings _settings;

        /// <summary>
        /// Initializes a new instance of the PaymentGatewayFactory
        /// </summary>
        /// <param name="settings">Payment gateway configuration settings</param>
        /// <exception cref="ArgumentNullException">Thrown when settings is null</exception>
        public PaymentGatewayFactory(PaymentGatewaySettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Creates a payment gateway instance based on the configured provider
        /// </summary>
        /// <returns>Payment gateway instance</returns>
        /// <exception cref="InvalidOperationException">Thrown when provider settings are not configured</exception>
        /// <exception cref="NotSupportedException">Thrown when provider is not supported</exception>
        public IPaymentGateway CreateGateway()
        {
            return _settings.Provider.ToLower() switch
            {
                "stripe" => _settings.Stripe is not null
                    ? new StripePaymentGateway(_settings.Stripe.SecretKey, _settings.Stripe.PublishableKey, _settings.Stripe.ApiBaseUrl)
                    : throw new InvalidOperationException("Stripe settings are not configured"),

                "paypal" => _settings.PayPal is not null
                    ? new PayPalPaymentGateway(_settings.PayPal.ClientId, _settings.PayPal.ClientSecret, _settings.PayPal.UseSandbox)
                    : throw new InvalidOperationException("PayPal settings are not configured"),

                "square" => _settings.Square is not null
                    ? new SquarePaymentGateway(_settings.Square.AccessToken, _settings.Square.LocationId, _settings.Square.UseSandbox)
                    : throw new InvalidOperationException("Square settings are not configured"),

                "adyen" => _settings.Adyen is not null
                    ? new AdyenPaymentGateway(_settings.Adyen.ApiKey, _settings.Adyen.MerchantAccount, _settings.Adyen.UseTestMode)
                    : throw new InvalidOperationException("Adyen settings are not configured"),

                "braintree" => _settings.Braintree is not null
                    ? new BraintreePaymentGateway(_settings.Braintree.MerchantId, _settings.Braintree.PublicKey, _settings.Braintree.PrivateKey, _settings.Braintree.UseSandbox)
                    : throw new InvalidOperationException("Braintree settings are not configured"),

                "worldpay" => _settings.Worldpay is not null
                    ? new WorldpayPaymentGateway(_settings.Worldpay.ServiceKey, _settings.Worldpay.UseTestMode)
                    : throw new InvalidOperationException("Worldpay settings are not configured"),

                "authorizenet" => _settings.AuthorizeNet is not null
                    ? new AuthorizeNetPaymentGateway(_settings.AuthorizeNet.ApiLoginId, _settings.AuthorizeNet.TransactionKey, _settings.AuthorizeNet.UseSandbox)
                    : throw new InvalidOperationException("Authorize.Net settings are not configured"),

                "klarna" => _settings.Klarna is not null
                    ? new KlarnaPaymentGateway(_settings.Klarna.Username, _settings.Klarna.Password, _settings.Klarna.UseTestMode)
                    : throw new InvalidOperationException("Klarna settings are not configured"),

                "mollie" => _settings.Mollie is not null
                    ? new MolliePaymentGateway(_settings.Mollie.ApiKey, _settings.Mollie.UseTestMode)
                    : throw new InvalidOperationException("Mollie settings are not configured"),

                "checkoutcom" => _settings.CheckoutCom is not null
                    ? new CheckoutComPaymentGateway(_settings.CheckoutCom.SecretKey, _settings.CheckoutCom.UseSandbox)
                    : throw new InvalidOperationException("Checkout.com settings are not configured"),

                "payu" => _settings.PayU is not null
                    ? new PayUPaymentGateway(_settings.PayU.PosId, _settings.PayU.ClientSecret, _settings.PayU.UseSandbox)
                    : throw new InvalidOperationException("PayU settings are not configured"),

                "gocardless" => _settings.GoCardless is not null
                    ? new GoCardlessPaymentGateway(_settings.GoCardless.AccessToken, _settings.GoCardless.UseSandbox)
                    : throw new InvalidOperationException("GoCardless settings are not configured"),

                "trustly" => _settings.Trustly is not null
                    ? new TrustlyPaymentGateway(_settings.Trustly.Username, _settings.Trustly.Password, _settings.Trustly.UseTestMode)
                    : throw new InvalidOperationException("Trustly settings are not configured"),

                "vipps" => _settings.Vipps is not null
                    ? new VippsPaymentGateway(_settings.Vipps.ClientId, _settings.Vipps.SubscriptionKey, _settings.Vipps.UseTestMode)
                    : throw new InvalidOperationException("Vipps settings are not configured"),

                "nets" => _settings.Nets is not null
                    ? new NetsPaymentGateway(_settings.Nets.SecretKey, _settings.Nets.UseTestMode)
                    : throw new InvalidOperationException("Nets settings are not configured"),

                "elavon" => _settings.Elavon is not null
                    ? new ElavonPaymentGateway(_settings.Elavon.MerchantId, _settings.Elavon.UserId, _settings.Elavon.UseTestMode)
                    : throw new InvalidOperationException("Elavon settings are not configured"),

                "cybersource" => _settings.Cybersource is not null
                    ? new CybersourcePaymentGateway(_settings.Cybersource.MerchantId, _settings.Cybersource.ApiKeyId, _settings.Cybersource.UseTestMode)
                    : throw new InvalidOperationException("Cybersource settings are not configured"),

                "bitpay" => _settings.BitPay is not null
                    ? new BitPayPaymentGateway(_settings.BitPay.ApiToken, _settings.BitPay.UseTestMode)
                    : throw new InvalidOperationException("BitPay settings are not configured"),

                "opennode" => _settings.OpenNode is not null
                    ? new OpenNodePaymentGateway(_settings.OpenNode.ApiKey, _settings.OpenNode.UseTestMode)
                    : throw new InvalidOperationException("OpenNode settings are not configured"),

                "mpesa" => _settings.Mpesa is not null
                    ? new MpesaPaymentGateway(_settings.Mpesa.ConsumerKey, _settings.Mpesa.ConsumerSecret, _settings.Mpesa.Shortcode, _settings.Mpesa.Passkey, _settings.Mpesa.UseSandbox)
                    : throw new InvalidOperationException("M-Pesa settings are not configured"),

                "flutterwave" => _settings.Flutterwave is not null
                    ? new FlutterwavePaymentGateway(_settings.Flutterwave.PublicKey, _settings.Flutterwave.SecretKey, _settings.Flutterwave.EncryptionKey, _settings.Flutterwave.UseTestMode)
                    : throw new InvalidOperationException("Flutterwave settings are not configured"),

                "pesapal" => _settings.Pesapal is not null
                    ? new PesapalPaymentGateway(_settings.Pesapal.ConsumerKey, _settings.Pesapal.ConsumerSecret, _settings.Pesapal.UseSandbox)
                    : throw new InvalidOperationException("Pesapal settings are not configured"),

                "paystack" => _settings.Paystack is not null
                    ? new PaystackPaymentGateway(_settings.Paystack.PublicKey, _settings.Paystack.SecretKey, _settings.Paystack.UseTestMode)
                    : throw new InvalidOperationException("Paystack settings are not configured"),

                "ipayafrica" => _settings.IPayAfrica is not null
                    ? new IPayAfricaPaymentGateway(_settings.IPayAfrica.VendorId, _settings.IPayAfrica.HashKey, _settings.IPayAfrica.UseTestMode)
                    : throw new InvalidOperationException("iPay Africa settings are not configured"),

                "dpogroup" => _settings.DpoGroup is not null
                    ? new DpoGroupPaymentGateway(_settings.DpoGroup.CompanyToken, _settings.DpoGroup.ServiceType, _settings.DpoGroup.UseTestMode)
                    : throw new InvalidOperationException("DPO Group settings are not configured"),

                "intasend" => _settings.IntaSend is not null
                    ? new IntaSendPaymentGateway(_settings.IntaSend.PublishableKey, _settings.IntaSend.SecretKey, _settings.IntaSend.UseTestMode)
                    : throw new InvalidOperationException("IntaSend settings are not configured"),

                "jambopay" => _settings.JamboPay is not null
                    ? new JamboPayPaymentGateway(_settings.JamboPay.MerchantId, _settings.JamboPay.ApiKey, _settings.JamboPay.UseTestMode)
                    : throw new InvalidOperationException("JamboPay settings are not configured"),

                "kopokopo" => _settings.KopoKopo is not null
                    ? new KopoKopoPaymentGateway(_settings.KopoKopo.ClientId, _settings.KopoKopo.ClientSecret, _settings.KopoKopo.ApiKey, _settings.KopoKopo.UseSandbox)
                    : throw new InvalidOperationException("KopoKopo settings are not configured"),

                "africastalking" => _settings.AfricasTalking is not null
                    ? new AfricasTalkingPaymentGateway(_settings.AfricasTalking.Username, _settings.AfricasTalking.ApiKey, _settings.AfricasTalking.ProductName, _settings.AfricasTalking.UseSandbox)
                    : throw new InvalidOperationException("Africa's Talking settings are not configured"),

                _ => throw new NotSupportedException($"Payment gateway provider '{_settings.Provider}' is not supported")
            };
        }

        /// <summary>
        /// Creates a payment gateway instance for a specific provider
        /// </summary>
        /// <param name="providerCode">Provider code (stripe, paypal, square)</param>
        /// <returns>Payment gateway instance</returns>
        /// <exception cref="InvalidOperationException">Thrown when provider settings are not configured</exception>
        /// <exception cref="NotSupportedException">Thrown when provider is not supported</exception>
        public IPaymentGateway CreateGateway(string providerCode)
        {
            var originalProvider = _settings.Provider;
            try
            {
                _settings.Provider = providerCode;
                return CreateGateway();
            }
            finally
            {
                _settings.Provider = originalProvider;
            }
        }
    }
}
