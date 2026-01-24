namespace PaymentGatewayLib.Infrastructure.Settings
{
    /// <summary>
    /// Configuration settings for DPO Group (DPO Pay) payment gateway
    /// </summary>
    public class DpoGroupSettings
    {
        /// <summary>
        /// DPO company token
        /// </summary>
        public string CompanyToken { get; set; } = string.Empty;

        /// <summary>
        /// DPO service type
        /// </summary>
        public string ServiceType { get; set; } = string.Empty;

        /// <summary>
        /// Whether to use test mode
        /// </summary>
        public bool UseTestMode { get; set; } = true;

        /// <summary>
        /// Payment URL endpoint
        /// </summary>
        public string PaymentUrl { get; set; } = string.Empty;

        /// <summary>
        /// Callback URL for payment notifications
        /// </summary>
        public string CallbackUrl { get; set; } = string.Empty;
    }
}
