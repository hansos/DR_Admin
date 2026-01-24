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
                    ? new StripePaymentGateway(
                        _settings.Stripe.SecretKey,
                        _settings.Stripe.PublishableKey,
                        _settings.Stripe.ApiBaseUrl
                    )
                    : throw new InvalidOperationException("Stripe settings are not configured"),

                "paypal" => _settings.PayPal is not null
                    ? new PayPalPaymentGateway(
                        _settings.PayPal.ClientId,
                        _settings.PayPal.ClientSecret,
                        _settings.PayPal.UseSandbox
                    )
                    : throw new InvalidOperationException("PayPal settings are not configured"),

                "square" => _settings.Square is not null
                    ? new SquarePaymentGateway(
                        _settings.Square.AccessToken,
                        _settings.Square.LocationId,
                        _settings.Square.UseSandbox
                    )
                    : throw new InvalidOperationException("Square settings are not configured"),

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
