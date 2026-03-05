using PaymentGatewayLib.Implementations;
using PaymentGatewayLib.Interfaces;

namespace PaymentGatewayLib.Factories
{
    /// <summary>
    /// Creates payment gateway clients from runtime gateway credentials.
    /// </summary>
    public static class PaymentGatewayRuntimeFactory
    {
        /// <summary>
        /// Creates a payment gateway client for a provider.
        /// </summary>
        /// <param name="providerCode">Provider code (stripe, paypal, square).</param>
        /// <param name="apiKey">Gateway API key.</param>
        /// <param name="apiSecret">Gateway API secret.</param>
        /// <param name="useSandbox">Whether to use sandbox mode.</param>
        /// <returns>Gateway client instance.</returns>
        public static IPaymentGateway Create(string providerCode, string apiKey, string apiSecret, bool useSandbox)
        {
            var provider = (providerCode ?? string.Empty).Trim().ToLowerInvariant();

            return provider switch
            {
                "stripe" => new StripePaymentGateway(apiSecret, apiKey),
                "paypal" => new PayPalPaymentGateway(apiKey, apiSecret, useSandbox),
                "square" => new SquarePaymentGateway(apiKey, apiSecret, useSandbox),
                _ => throw new NotSupportedException($"Payment provider '{providerCode}' is not currently supported")
            };
        }
    }
}
